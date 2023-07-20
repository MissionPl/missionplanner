using AltitudeAngelWings.Models;
using System.Collections.Generic;

namespace AltitudeAngelWings.Extra
{
    public interface IMissionPlannerState
    {
        bool IsArmed { get; }
        double Longitude { get; }
        double Latitude { get; }
        float Altitude { get; }
        float GroundSpeed { get; }
        float GroundCourse { get; }
        bool IsConnected { get; }
        float VerticalSpeed { get; }
        FlightCapability FlightCapability { get; }
        IList<FlightPlanWaypoint> Waypoints { get; }
        FlightPlanWaypoint HomeLocation { get; }
    }
}
