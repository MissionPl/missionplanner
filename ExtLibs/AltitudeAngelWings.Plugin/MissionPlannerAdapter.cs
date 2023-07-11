using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using MissionPlanner;
using MissionPlanner.Utilities;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NodaTime;
using Resources = AltitudeAngelWings.Plugin.Properties.Resources;

namespace AltitudeAngelWings.Plugin
{
    public class MissionPlannerAdapter : IMissionPlanner
    {
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly Func<IList<Locationwp>> _getFlightPlan;
        private readonly ISettings _settings;
        public IMap FlightPlanningMap { get; }
        public IMap FlightDataMap { get; }
        public ProductInfoHeaderValue VersionHeader { get; }

        public MissionPlannerAdapter(IUiThreadInvoke uiThreadInvoke, IMap flightDataMap, IMap flightPlanningMap, Func<IList<Locationwp>> getFlightPlan, ISettings settings, string titleVersionString)
        {
            FlightDataMap = flightDataMap;
            FlightPlanningMap = flightPlanningMap;
            _uiThreadInvoke = uiThreadInvoke;
            _getFlightPlan = getFlightPlan;
            _settings = settings;
            var lastPart = titleVersionString.LastIndexOf(' ');
            if (lastPart > 0 && lastPart < titleVersionString.Length - 1)
            {
                titleVersionString = titleVersionString.Substring(lastPart + 1);
            }

            try
            {
                VersionHeader = new ProductInfoHeaderValue("MissionPlanner", titleVersionString);
            }
            catch (FormatException)
            {
                VersionHeader = new ProductInfoHeaderValue("MissionPlanner", "unknown");
            }
        }

        public Task<FlightPlan> GetFlightPlan()
        {
            var waypoints = _getFlightPlan().Where(IsValidWaypoint).ToList();
            if (waypoints.Count == 0)
            {
                return null;
            }
            var homeLocation = MainV2.comPort.MAV.cs.PlannedHomeLocation;
            waypoints.Insert(0, new Locationwp
            {
                lat = homeLocation.Lat,
                lng = homeLocation.Lng,
                alt = (float)homeLocation.Alt
            });
            var envelope = GeometryFactory.Default.CreateMultiPoint(
                waypoints
                    .Select(l => new Point(l.lng, l.lat))
                    .ToArray())
                .Envelope;
            var center = envelope.Centroid;
            var minimumBoundingCircle = new MinimumBoundingCircle(envelope);
            return Task.FromResult(new FlightPlan(waypoints.Select(f => new FlightPlanWaypoint
            {
                Latitude = f.lat,
                Longitude = f.lng,
                Altitude = (int)f.alt,
            }))
            {
                CenterLongitude = center.X,
                CenterLatitude = center.Y,
                BoundingRadius = (int)Math.Ceiling(minimumBoundingCircle.GetRadius()),
                FlightCapability = MavTypeToFlightCapability(MainV2.comPort.MAV.aptype),
                Summary =  _settings.FlightReportName,
                Description = _settings.FlightReportDescription,
                Duration = Duration.FromTimeSpan(_settings.FlightReportTimeSpan),
                UseLocalConflictScope = _settings.UseFlightPlanLocalScope,
                AllowSmsContact = false,
                SmsPhoneNumber = "",
                DroneSerialNumber = "",
                FlightOperationMode = FlightOperationMode.BVLOS
            });
        }

        public Task CommandDroneToReturnToBase()
            => SetMode("RTL");

        public Task CommandDroneToLand(
            float latitude,
            float longitude)
            => SetMode("Land");

        public Task CommandDroneToLoiter(
            float latitude,
            float longitude,
            float altitude)
            => SetMode("Loiter");

        public Task CommandDroneAllClear()
            => SetMode("Auto");

        private static Task SetMode(string mode)
        {
            MainV2.comPort.setMode(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, mode);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task NotifyConflict(string message)
            => ShowMessageBox(message, Resources.MissionPlannerAdapterNotifyMessageTitle);

        /// <inheritdoc />
        public Task NotifyConflictResolved(string message)
            => ShowMessageBox(message, Resources.MissionPlannerAdapterNotifyMessageTitle);

        /// <inheritdoc />
        public Task Disarm()
            => MainV2.comPort.doARMAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, false);

        public Task ShowMessageBox(string message, string caption = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Resources.MissionPlannerAdapterMessageBoxDefaultCaption;
            }
            return _uiThreadInvoke.Invoke(() => CustomMessageBox.Show(message, caption));
        }

        public Task<bool> ShowYesNoMessageBox(string message, string caption = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Resources.MissionPlannerAdapterMessageBoxDefaultCaption;
            }
            return _uiThreadInvoke.Invoke(() =>
                CustomMessageBox.Show(message, caption, MessageBoxButtons.YesNo) == (int)DialogResult.Yes);
        }

        private static bool IsValidWaypoint(Locationwp location)
        {
            // Command that can contain latitude and longitude
            var cmd = (MAVLink.MAV_CMD)location.id;
            if (cmd != MAVLink.MAV_CMD.WAYPOINT &&
                cmd != MAVLink.MAV_CMD.LOITER_UNLIM &&
                cmd != MAVLink.MAV_CMD.LOITER_TURNS &&
                cmd != MAVLink.MAV_CMD.LOITER_TIME &&
                cmd != MAVLink.MAV_CMD.LAND &&
                cmd != MAVLink.MAV_CMD.TAKEOFF &&
                cmd != MAVLink.MAV_CMD.FOLLOW &&
                cmd != MAVLink.MAV_CMD.LOITER_TO_ALT &&
                cmd != MAVLink.MAV_CMD.PATHPLANNING &&
                cmd != MAVLink.MAV_CMD.SPLINE_WAYPOINT &&
                cmd != MAVLink.MAV_CMD.VTOL_TAKEOFF &&
                cmd != MAVLink.MAV_CMD.VTOL_LAND &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PLACE &&
                cmd != MAVLink.MAV_CMD.DO_SET_HOME &&
                cmd != MAVLink.MAV_CMD.DO_LAND_START &&
                cmd != MAVLink.MAV_CMD.DO_REPOSITION &&
                cmd != MAVLink.MAV_CMD.DO_SET_ROI_LOCATION &&
                cmd != MAVLink.MAV_CMD.DO_MOUNT_CONTROL &&
                cmd != MAVLink.MAV_CMD.OVERRIDE_GOTO &&
                cmd != MAVLink.MAV_CMD.SET_GUIDED_SUBMODE_CIRCLE &&
                cmd != MAVLink.MAV_CMD.FENCE_RETURN_POINT &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.RALLY_POINT &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PREPARE_DEPLOY &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_1 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_2 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_3 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_4 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_5 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_1 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_2 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_3 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_4 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_5)
            {
                return false;
            }

            // Exclude "empty" latitude and longitude
            return location.lat != 0 && location.lng != 0;
        }

        private static FlightCapability MavTypeToFlightCapability(MAVLink.MAV_TYPE mavType)
        {
            switch (mavType)
            {
                case MAVLink.MAV_TYPE.FIXED_WING:
                    return FlightCapability.FixedWing;

                case MAVLink.MAV_TYPE.QUADROTOR:
                case MAVLink.MAV_TYPE.COAXIAL:
                case MAVLink.MAV_TYPE.HEXAROTOR:
                case MAVLink.MAV_TYPE.OCTOROTOR:
                case MAVLink.MAV_TYPE.TRICOPTER:
                case MAVLink.MAV_TYPE.DODECAROTOR:
                case MAVLink.MAV_TYPE.HELICOPTER:
                    return FlightCapability.Rotary;

                case MAVLink.MAV_TYPE.VTOL_DUOROTOR:
                case MAVLink.MAV_TYPE.VTOL_QUADROTOR:
                case MAVLink.MAV_TYPE.VTOL_TILTROTOR:
                case MAVLink.MAV_TYPE.VTOL_RESERVED2:
                case MAVLink.MAV_TYPE.VTOL_RESERVED3:
                case MAVLink.MAV_TYPE.VTOL_RESERVED4:
                case MAVLink.MAV_TYPE.VTOL_RESERVED5:
                    return FlightCapability.VTOL;

                default:
                    return FlightCapability.Unspecified;
            }
        }
    }
}