using System;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.FlightData.Providers
{
    public class MissionPlannerFlightDataProvider : IFlightDataProvider
    {
        private const int GeographicPrecision = 7;
        private const int AltitudePrecision = 2;

        public MissionPlannerFlightDataProvider(IMissionPlannerState missionPlannerState)
        {
            _missionPlannerState = missionPlannerState;
        }

        public Models.FlightData GetCurrentFlightData()
        {
            return new Models.FlightData
            {
                Armed = _missionPlannerState.IsArmed,
                CurrentPosition = new FlightDataPosition
                {
                    Longitude = Math.Round(_missionPlannerState.Longitude, GeographicPrecision, MidpointRounding.AwayFromZero),
                    Latitude = Math.Round(_missionPlannerState.Latitude, GeographicPrecision, MidpointRounding.AwayFromZero),
                    Altitude = Math.Round(_missionPlannerState.Altitude, AltitudePrecision, MidpointRounding.AwayFromZero),
                    Course = _missionPlannerState.GroundCourse,
                    Speed = _missionPlannerState.GroundSpeed,
                    VerticalSpeed = _missionPlannerState.VerticalSpeed
                }
            };
        }

        private readonly IMissionPlannerState _missionPlannerState;
    }
}
