﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service;
using Newtonsoft.Json.Linq;
using Polly;

namespace AltitudeAngelWings.Plugin
{
    public class ExternalWebBrowserAuthorizeCodeProvider : IAuthorizeCodeProvider
    {
        private readonly ISettings _settings;
        private readonly IAsyncPolicy _asyncPolicy;
        private readonly ProductInfoHeaderValue _version;
        private string _pollId;

        public ExternalWebBrowserAuthorizeCodeProvider(ISettings settings, IAsyncPolicy asyncPolicy, ProductInfoHeaderValue version)
        {
            _settings = settings;
            _asyncPolicy = asyncPolicy;
            _version = version;
        }

        public void GetAuthorizeParameters(NameValueCollection parameters)
        {
            // Generate a random poll id
            _pollId = Guid.NewGuid().ToString("N");

            // Add poll id to the parameters
            parameters.Add("poll_id", _pollId);
        }

        public Task<string> GetAuthorizeCode(Uri authorizeUri)
        {
            if (string.IsNullOrWhiteSpace(_settings.ClientId) || string.IsNullOrWhiteSpace(_settings.ClientSecret))
            {
                throw new InvalidOperationException("ClientId and ClientSecret not set correctly.");
            }

            var authorizeCode = UiTask.ShowDialog(async cancellationToken =>
                {
                    using (var client = new HttpClient())
                    {
                        var token = await GetClientTokenForLoginPoll(client, cancellationToken);
                        Process.Start(authorizeUri.ToString());

                        string code;
                        do
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                            code = await PollForAuthenticationCode(client, token, cancellationToken);
                        } while (!cancellationToken.IsCancellationRequested && code == null);

                        return code;
                    }
                },
                "Opening a browser to login to Altitude Angel. Please login in the browser.");
            if (authorizeCode == null)
            {
                throw new TaskCanceledException("User cancelled getting an authorize code.");
            }
            return Task.FromResult(authorizeCode);
        }

        private async Task<string> GetClientTokenForLoginPoll(HttpMessageInvoker client, CancellationToken cancellationToken)
        {
            using (var response = await _asyncPolicy.ExecuteAsync(() => client.SendAsync(new HttpRequestMessage
                   {
                       Method = HttpMethod.Post,
                       RequestUri = new Uri($"{_settings.AuthenticationUrl}/oauth/v2/token"),
                       Headers = { UserAgent = { _version } },
                       Content = new FormUrlEncodedContent(new[]
                       {
                           new KeyValuePair<string, string>("client_id", _settings.ClientId),
                           new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                           new KeyValuePair<string, string>("grant_type", "client_credentials"),
                           new KeyValuePair<string, string>("token_type", "jwt"),
                       })
                   }, cancellationToken)))
            {
                response.EnsureSuccessStatusCode();
                return (string)JObject.Parse(await response.Content.ReadAsStringAsync())["access_token"];
            };
        }

        private async Task<string> PollForAuthenticationCode(HttpMessageInvoker client, string token, CancellationToken cancellationToken)
        {
            using (var response = await _asyncPolicy.ExecuteAsync(() => client.SendAsync(new HttpRequestMessage
                   {
                       Method = HttpMethod.Get,
                       RequestUri = new Uri($"{_settings.AuthenticationUrl}/api/v1/security/get-login?id={_pollId}"),
                       Headers =
                       {
                           Authorization = new AuthenticationHeaderValue("Bearer", token),
                           UserAgent = { _version }
                       }
                   }, cancellationToken)))
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return (string)JObject.Parse(await response.Content.ReadAsStringAsync())["code"];
            }
        }
    }
}