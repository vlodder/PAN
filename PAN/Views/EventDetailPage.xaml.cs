using Mapsui;
using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using PAN.ViewModels;

namespace PAN.Views;

public partial class EventDetailPage : ContentPage
{
    private readonly EventDetailViewModel _viewModel;
    private MapControl? _mapControl;
    private MemoryLayer? _pinLayer;
    private bool _mapInitialized;

    public EventDetailPage(EventDetailViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventDetailViewModel.Evenement))
        {
            MainThread.BeginInvokeOnMainThread(ChargerPosition);
        }
    }

    private void InitialiserCarte()
    {
        if (_mapInitialized)
            return;

        _mapControl = new MapControl
        {
            Map = new Mapsui.Map()
        };

        _mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        MapHost.Content = _mapControl;
        _mapInitialized = true;
    }

    private void ChargerPosition()
    {
        if (_viewModel.Evenement?.Latitude == null ||
            _viewModel.Evenement.Longitude == null)
            return;

        InitialiserCarte();

        double latitude = Convert.ToDouble(_viewModel.Evenement.Latitude.Value);
        double longitude = Convert.ToDouble(_viewModel.Evenement.Longitude.Value);

        AfficherMarqueur(latitude, longitude);
        CentrerCarte(latitude, longitude);
    }

    private void AfficherMarqueur(double latitude, double longitude)
    {
        if (_mapControl?.Map == null)
            return;

        var spherical = SphericalMercator.FromLonLat(longitude, latitude);

        var feature = new PointFeature(spherical.x, spherical.y)
        {
            Styles =
            [
                new SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 2),
                    SymbolScale = 0.8
                }
            ]
        };

        _pinLayer ??= new MemoryLayer
        {
            Name = "ObservationLayer"
        };

        _pinLayer.Features = new[] { feature };

        if (!_mapControl.Map.Layers.Contains(_pinLayer))
            _mapControl.Map.Layers.Add(_pinLayer);

        _mapControl.Refresh();
    }

    private void CentrerCarte(double latitude, double longitude)
    {
        if (_mapControl?.Map?.Navigator == null)
            return;

        var spherical = SphericalMercator.FromLonLat(longitude, latitude);

        _mapControl.Map.Navigator.CenterOnAndZoomTo(
            new MPoint(spherical.x, spherical.y),
            _mapControl.Map.Navigator.Resolutions[14]);

        _mapControl.Refresh();
    }
}