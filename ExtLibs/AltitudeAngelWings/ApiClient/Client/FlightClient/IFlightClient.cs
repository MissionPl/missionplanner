﻿using AltitudeAngelWings.ApiClient.Models.FlightV2;
using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models.Flight;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public interface IFlightClient : IDisposable
    {
        Task CompleteFlight(string flightId, CancellationToken cancellationToken = default);
        Task<CreateFlightPlanResponse> CreateFlightPlan(CreateFlightPlanRequest flightPlan, CancellationToken cancellationToken = default);
        Task<StartFlightResponse> StartFlight(StartFlightRequest startFlightRequest, CancellationToken cancellationToken = default);
        Task CompleteFlightPlan(string flightPlanId, CancellationToken cancellationToken = default);
        Task CancelFlightPlan(string flightPlanId, CancellationToken cancellationToken = default);
        Task AcceptInstruction(string instructionId, CancellationToken cancellationToken = default);
        Task RejectInstruction(string instructionId, CancellationToken cancellationToken = default);
    }
}
