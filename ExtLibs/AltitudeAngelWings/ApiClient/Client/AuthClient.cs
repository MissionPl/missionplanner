using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class AuthClient : IAuthClient
    {
        private readonly ISettings _settings;
        private readonly IFlurlClient _client;

        public AuthClient(ISettings settings, IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory
                }
            };
        }

        public Task<TokenResponse> GetTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "refresh_token",
                        refresh_token = refreshToken,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public Task<TokenResponse> GetTokenFromAuthorizationCode(string authorizationCode, CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "authorization_code",
                        code = authorizationCode,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public Task<TokenResponse> GetTokenFromClientCredentials(CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        grant_type = "client_credentials",
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public async Task<string> GetAuthorizationCode(string accessToken, string pollId, CancellationToken cancellationToken)
        {
            var response = await _settings.AuthenticationUrl
                .AppendPathSegments("api", "v1", "security", "get-login")
                .SetQueryParam("id", pollId)
                .WithHeader("Authorization", $"Bearer {accessToken}")
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .WithClient(_client)
                .GetAsync(cancellationToken);

            if (response.StatusCode != (int)HttpStatusCode.OK)
            {
                return null;
            }

            var data = await response.GetJsonAsync();
            return data.code;
        }
    }
}