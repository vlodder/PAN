using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using PAN.context.Models;
using PAN.Models;
using PAN.Services;
using SkiaSharp;

namespace PAN.ViewModels;

public partial class AdminViewModel(
    IDialogService dialogService,
    INavigationService navigationService,
    GeipanContext context,
    IEvenementService evenementService)
    : BaseViewModel(dialogService, navigationService)
{
    private readonly GeipanContext _context = context;
    private readonly IEvenementService _evenementService = evenementService;

    public SolidColorPaint LegendTextPaint { get; } = new(SKColors.White);

    [ObservableProperty]
    private ISeries[] series = [];

    [ObservableProperty]
    private int totalCases;

    [ObservableProperty]
    private ObservableCollection<EvenementListItem> evenements = [];

    public async Task RefreshDataAsync()
    {
        await LoadDataAsync();
        await LoadEvenementsAsync();
    }

    private async Task LoadEvenementsAsync()
    {
        try
        {
            var items = await _context.Evenement
                .AsNoTracking()
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .OrderByDescending(e => e.DateHeureObservation)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    Ville = e.IdLocalisationNavigation != null && e.IdLocalisationNavigation.Ville != null
                        ? e.IdLocalisationNavigation.Ville
                        : "Inconnue",
                    Descriptif = e.Descriptif ?? string.Empty,
                    TypeNom = e.IdTypeNavigation != null && e.IdTypeNavigation.Nom != null
                        ? e.IdTypeNavigation.Nom
                        : "Inconnu",
                    EstMouvant = e.Estmouvant,
                    DateHeureObservation = e.DateHeureObservation,
                    UpVote = e.UpVote ?? 0,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    IdType = e.IdType
                })
                .ToListAsync();

            Evenements = new ObservableCollection<EvenementListItem>(items);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible de charger les observations : {ex.Message}",
                "OK");
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var stats = await _context.Evenement
                .AsNoTracking()
                .Include(e => e.IdClassementNavigation)
                .GroupBy(e => e.IdClassementNavigation != null && e.IdClassementNavigation.Nom != null
                    ? e.IdClassementNavigation.Nom
                    : "Non classé")
                .Select(g => new
                {
                    Classification = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            TotalCases = stats.Sum(s => s.Count);

            var colors = new[]
            {
                SKColor.Parse("#4ED464"),
                SKColor.Parse("#5D9BFF"),
                SKColor.Parse("#A855F7"),
                SKColor.Parse("#F59E0B"),
                SKColor.Parse("#EF4444"),
                SKColor.Parse("#14B8A6"),
                SKColor.Parse("#E879F9")
            };

            Series = stats.Select((s, index) => new PieSeries<int>
            {
                Name = $"{s.Classification} : {s.Count} cas",
                Values = new[] { s.Count },

                Fill = new SolidColorPaint(colors[index % colors.Length]),

                Stroke = new SolidColorPaint(SKColor.Parse("#07172A"))
                {
                    StrokeThickness = 5
                },

                InnerRadius = 80,
                MaxRadialColumnWidth = 85,
                HoverPushout = 10,

                DataLabelsPaint = null
            }).ToArray();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible de charger les statistiques : {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task OpenDetailAsync(EvenementListItem item)
    {
        if (item == null)
            return;

        await Shell.Current.GoToAsync($"EventDetailPage?idEvenement={item.IdEvenement}");
    }

    [RelayCommand]
    private async Task EditEvenementAsync(EvenementListItem item)
    {
        if (item == null)
            return;

        await Shell.Current.GoToAsync($"NewEventPage?idEvenement={item.IdEvenement}");
    }

    [RelayCommand]
    private async Task DeleteEvenementAsync(EvenementListItem item)
    {
        if (item == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Suppression",
            $"Supprimer l'observation de {item.Ville} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        try
        {
            var evenement = await _context.Evenement
                .FirstOrDefaultAsync(e => e.IdEvenement == item.IdEvenement);

            if (evenement == null)
                return;

            _context.Evenement.Remove(evenement);
            await _context.SaveChangesAsync();

            await RefreshDataAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible de supprimer l'événement : {ex.Message}",
                "OK");
        } }
        public SolidColorPaint TooltipTextPaint { get; } = new(SKColors.White);
}
