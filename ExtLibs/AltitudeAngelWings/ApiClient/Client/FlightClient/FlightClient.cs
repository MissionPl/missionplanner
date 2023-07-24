﻿using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests;
using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.Service;
using System.Threading;
using AltitudeAngelWings.ApiClient.Models.Flight;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public class FlightClient : IFlightClient
    {
        private readonly ISettings _settings;
        private readonly IFlurlClient _client;

        public FlightClient(ISettings settings, IHttpClientFactory clientFactory, ISerializer serializer)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                    JsonSerializer = serializer
                }
            };
        }

        public Task<CreateFlightPlanResponse> CreateFlightPlan(CreateFlightPlanRequest flightPlan, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flightapprovals")
                .WithClient(_client)
                .PostJsonAsync(flightPlan, cancellationToken)
                .ReceiveJson<CreateFlightPlanResponse>();

        public Task<StartFlightResponse> StartFlight(StartFlightRequest startFlightRequest, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights")
                .WithClient(_client)
                .PostJsonAsync(startFlightRequest, cancellationToken)
                .ReceiveJson<StartFlightResponse>();

        public Task CompleteFlight(string flightId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights", flightId)
                .WithClient(_client)
                .DeleteAsync(cancellationToken);

        public Task CompleteFlightPlan(string flightPlanId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flightapprovals", flightPlanId, "complete")
                .WithClient(_client)
                .PostAsync(cancellationToken: cancellationToken);

        public Task CancelFlightPlan(string flightPlanId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flightapprovals", flightPlanId, "cancel")
                .WithClient(_client)
                .PostAsync(cancellationToken: cancellationToken);

        public Task AcceptInstruction(string instructionId, CancellationToken cancellationToken = default) => ProcessInstruction(instructionId, true, cancellationToken);

        public Task RejectInstruction(string instructionId, CancellationToken cancellationToken = default) => ProcessInstruction(instructionId, false, cancellationToken);

        private Task ProcessInstruction(string instructionId, bool accept, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "instructions", instructionId, accept ? "accept" : "reject")
                .WithClient(_client)
                .PutAsync(cancellationToken: cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
