using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using GMap.NET;
using GMap.NET.WindowsForms;
using Feature = GeoJSON.Net.Feature.Feature;
using Unit = System.Reactive.Unit;

namespace AltitudeAngelWings.Plugin
{
    internal class MapAdapter : IMap, IDisposable
    {
        private const int MinimumZoomLevelFilter = 10;

        private readonly GMapControl _mapControl;
        private readonly Func<bool> _enabled;
        private readonly IMapInfoDockPanel _mapInfoDockPanel;
        private readonly bool _ctrlForPanel;
        private CompositeDisposable _disposer = new CompositeDisposable();
        private readonly SynchronizationContext _context;
        private Point? _mouseDown;

        public IObservable<Unit> MapChanged { get; }
        public ObservableProperty<Feature> FeatureClicked { get; } = new ObservableProperty<Feature>(0);

        public MapAdapter(GMapControl mapControl, Func<bool> enabled, IMapInfoDockPanel mapInfoDockPanel, ISettings settings, bool ctrlForPanel)
        {
            _context = new WindowsFormsSynchronizationContext();
            _mapControl = mapControl;
            _enabled = enabled;
            _mapInfoDockPanel = mapInfoDockPanel;
            _ctrlForPanel = ctrlForPanel;

            var positionChanged = Observable
                .FromEvent<PositionChanged, PointLatLng>(
                    action => point => action(point),
                    handler => _mapControl.OnPositionChanged += handler,
                    handler => _mapControl.OnPositionChanged -= handler)
                .Select(i => Unit.Default);

            var mapZoom = Observable
                .FromEvent<MapZoomChanged, Unit>(
                    action => () => action(Unit.Default),
                    handler => _mapControl.OnMapZoomChanged += handler,
                    handler => _mapControl.OnMapZoomChanged -= handler);

            var visible = Observable
                .FromEvent<EventHandler, Unit>(
                    action => (s, e) => action(Unit.Default),
            handler => _mapControl.VisibleChanged += handler,
                    handler => _mapControl.VisibleChanged -= handler)
                .Where(i => _mapControl.Visible);

            var interval = Observable
                .Interval(TimeSpan.FromSeconds(settings.MapUpdateRefresh))
                .Select(_ => mapControl.Position)
                .Select(i => Unit.Default);

            MapChanged = visible
                .Merge(positionChanged)
                .Merge(interval)
                .Merge(mapZoom)
                .Throttle(TimeSpan.FromSeconds(settings.MapUpdateThrottle))
                .Where(i => _mapControl.Visible && _mapControl.Zoom >= MinimumZoomLevelFilter)
                .Where(i => settings.TokenResponse.IsValidForAuth())
                .ObserveOn(ThreadPoolScheduler.Instance);

            mapControl.MouseDown += OnMouseDown;
            mapControl.MouseUp += OnMouseUp;
        }

        private void OnMouseDown(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ((_ctrlForPanel && Control.ModifierKeys == Keys.Control) || !_ctrlForPanel))
            {
                _mouseDown = e.Location;
                return;
            }
            _mouseDown = null;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (_mouseDown == null || e.Location != _mouseDown)
            {
                _mouseDown = null;
                return;
            }

            var mapItems = GetMapItemsOnMouseClick(e.Location);
            if (mapItems.Length == 0)
            {
                _mapInfoDockPanel.Hide();
                return;
            }
            _mapInfoDockPanel.Show(mapItems.Select(i => i.GetFeatureProperties()).ToArray());
            foreach (var item in mapItems)
            {
                FeatureClicked.Value = item;
            }
        }

        public bool Enabled => _enabled();

        private Feature[] GetMapItemsOnMouseClick(Point point)
        {
            var mapItems = new List<Feature>();

            var polygons = _mapControl.Overlays
                .SelectMany(o => o.Polygons)
                .Where(p => p.IsVisible && p.IsHitTestVisible)
                .Where(p => p.IsInside(_mapControl.FromLocalToLatLng(point.X, point.Y)))
                .Select(p=> (Feature)p.Tag);
            mapItems.AddRange(polygons);

            var routes = _mapControl.Overlays
                .SelectMany(o => o.Routes)
                .Where(r => r.IsVisible && r.IsHitTestVisible)
                .Where(r =>
                {
                    var rp = new GPoint(point.X, point.Y);
                    rp.OffsetNegative(_mapControl.Core.renderOffset);
                    return r.IsInside((int)rp.X, (int)rp.Y);
                })
                .Select(r => (Feature)r.Tag);
            mapItems.AddRange(routes);

            return mapItems.ToArray();
        }

        public LatLong GetCenter()
        {
            var pointLatLng = default(PointLatLng);
            _context.Send(_ => pointLatLng = _mapControl.Position, null);
            return new LatLong(pointLatLng.Lat, pointLatLng.Lng);
        }

        public BoundingLatLong GetViewArea()
        {
            var rectLatLng = default(RectLatLng);
            _context.Send(_ => rectLatLng = _mapControl.ViewArea, null);
            if (rectLatLng.WidthLng < 0.03)
                rectLatLng.Inflate(0, (0.03 - rectLatLng.WidthLng) / 2);
            if (rectLatLng.HeightLat < 0.03)
                rectLatLng.Inflate((0.03 - rectLatLng.HeightLat) / 2, 0);

            return new BoundingLatLong
            {
                NorthEast = new LatLong(rectLatLng.Top, rectLatLng.Right),
                SouthWest = new LatLong(rectLatLng.Bottom, rectLatLng.Left)
            };
        }

        public void AddOverlay(string name)
            => _context.Send(state => _mapControl.Overlays.Add(new GMapOverlay(name)), null);

        public void DeleteOverlay(string name)
            => _context.Send(_ =>
            {
                var overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);
                if (overlay == null) return;
                _mapControl.Overlays.Remove(overlay);
                overlay.Dispose();
            }, null);

        public IOverlay GetOverlay(string name, bool createIfNotExists = false)
        {
            IOverlay result = null;
            _context.Send(_ =>
            {
                var overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);

                if (overlay == null)
                {
                    if (!createIfNotExists) throw new ArgumentException($"Overlay {name} not found.");
                    AddOverlay(name);
                    result = GetOverlay(name);
                    return;

                }

                result = new OverlayAdapter(overlay);
            }, null);
            return result;
        }

        public void Invalidate()
        {
            _mapControl.Invalidate();
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
                _disposer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}