using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.Services;

namespace PAN.ViewModels;

public partial class MapViewModel(
    IDialogService dialogService,
    INavigationService navigationService,
    IEvenementService evenementService)
    : BaseViewModel(dialogService, navigationService)
{
    private readonly IEvenementService _evenementService = evenementService;

    [ObservableProperty]
    private HtmlWebViewSource htmlSource = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        var events = await _evenementService.GetAllAsync();

        var points = events
            .Where(e => e.Latitude != null && e.Longitude != null)
            .Select(e => new
            {
                id = e.IdEvenement,
                ville = e.Ville,
                description = e.Descriptif,
                lat = e.Latitude,
                lng = e.Longitude,
                upvote = e.UpVote
            })
            .ToList();
        await Shell.Current.DisplayAlert(
    "Debug carte",
    $"Events: {events.Count} | Points GPS: {points.Count}",
    "OK");

        string json = JsonSerializer.Serialize(points);

        HtmlSource = new HtmlWebViewSource
        {
            Html = BuildMapHtml(json)
        };
    }

    private static string BuildMapHtml(string pointsJson)
    {
        return $$"""
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="https://unpkg.com/leaflet/dist/leaflet.css" />
    <script src="https://unpkg.com/leaflet/dist/leaflet.js"></script>

    <style>
        html, body, #map {
            height: 100%;
            width: 100%;
            margin: 0;
            background: #020B18;
        }
    </style>
</head>

<body>
    <div id="map"></div>

    <script>
        const map = L.map('map').setView([46.5, 2.5], 5);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19
        }).addTo(map);

        const points = {{pointsJson}};

        points.forEach(p => {
            L.marker([p.lat, p.lng])
                .addTo(map)
                .bindPopup(`
                    <b>${p.ville}</b><br>
                    ${p.description}<br><br>
                    <b>↑ ${p.upvote}</b>
                `);
        });
    </script>
</body>
</html>
""";
    }
}