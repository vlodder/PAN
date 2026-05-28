using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PAN.context.Models;
using PAN.Services;

namespace PAN.ViewModels;

public partial class MapViewModel : BaseViewModel
{
    private readonly IEvenementService _evenementService;

    private const int PageSize = 20;
    private int _skip = 0;
    private bool _isLoaded = false;

    private readonly List<Evenement> _loadedEvents = new();

    [ObservableProperty]
    private HtmlWebViewSource htmlSource = new();

    public MapViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IEvenementService evenementService)
        : base(dialogService, navigationService)
    {
        _evenementService = evenementService;
        GenerateMapHtml();
    }

    [RelayCommand]
    private async Task LoadMapAsync()
    {
        if (_isLoaded)
            return;

        _isLoaded = true;
        _skip = 0;
        _loadedEvents.Clear();

        await LoadMoreAsync();
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        try
        {
            var events = await _evenementService.GetEventsForMapAsync(_skip, PageSize);

            if (events.Count == 0)
            {
                await DialogService.ShowAlertAsync("Info", "Aucune observation supplémentaire.", "OK");
                return;
            }

            _loadedEvents.AddRange(events);
            _skip += PageSize;

            GenerateMapHtml();
        }
        catch (Exception ex)
        {
            await DialogService.ShowAlertAsync("Erreur", $"Impossible de charger la carte : {ex.Message}", "OK");
        }
    }

    private void GenerateMapHtml()
    {
        var markers = new StringBuilder();

        foreach (var ev in _loadedEvents)
        {
            if (ev.Latitude == null || ev.Longitude == null)
                continue;

            string lat = Convert.ToDouble(ev.Latitude).ToString(CultureInfo.InvariantCulture);
            string lng = Convert.ToDouble(ev.Longitude).ToString(CultureInfo.InvariantCulture);

            string titre = CleanJs(ev.Titre ?? "Observation");
            string descriptif = CleanJs(ev.Descriptif ?? "");

            markers.AppendLine($@"
                L.marker([{lat}, {lng}])
                    .addTo(map)
                    .bindPopup('<b>{titre}</b><br>{descriptif}');
            ");
        }

        HtmlSource = new HtmlWebViewSource
        {
            Html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />

    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>

    <style>
        html, body, #map {{
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
            background: #020B18;
        }}
    </style>
</head>

<body>
    <div id='map'></div>

    <script>
        var map = L.map('map', {{
            zoomControl: true
        }}).setView([46.603354, 1.888334], 5);

        L.tileLayer('https://tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            maxZoom: 18
        }}).addTo(map);

        {markers}
    </script>
</body>
</html>"
        };
    }

    private static string CleanJs(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\r", "")
            .Replace("\n", "<br>");
    }
}