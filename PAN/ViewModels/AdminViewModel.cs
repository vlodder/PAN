using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using PAN.context.Models;
using Microsoft.EntityFrameworkCore;
using PAN.Models;
using PAN.Services;

namespace PAN.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly GeipanContext _context;
    private readonly IEvenementService _evenementService;

    [ObservableProperty]
    private ISeries[] series;

    [ObservableProperty]
    private int totalCases;

    [ObservableProperty]
    private ObservableCollection<EvenementListItem> evenements = [];

    public AdminViewModel(GeipanContext context, IEvenementService evenementService)
    {
        _context = context;
        _evenementService = evenementService;
    }

    // Centralisation du rafraîchissement
    public async Task RefreshDataAsync()
    {
        LoadData();
        await LoadEvenementsAsync();
    }

    private async Task LoadEvenementsAsync()
    {
        try
        {
            // Utilisation de AsNoTracking pour optimiser la mémoire en lecture seule
            var items = await _context.Evenement
                .AsNoTracking()
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    Ville = e.IdLocalisationNavigation != null ? e.IdLocalisationNavigation.Ville : "Inconnue",
                    Descriptif = e.Descriptif,
                    TypeNom = e.IdTypeNavigation != null ? e.IdTypeNavigation.Nom : "Inconnu",
                    EstMouvant = e.Estmouvant,

                    // On gère les éventuels NULL de la base de données
                    DateHeureObservation = e.DateHeureObservation,
                    UpVote = e.UpVote ?? 0 // Attention au 'v' minuscule de e.Upvote
                })
                .ToListAsync();

            Evenements = new ObservableCollection<EvenementListItem>(items);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des observations : {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task OpenDetailAsync(EvenementListItem item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"EventDetailPage?idEvenement={item.IdEvenement}");
    }

    [RelayCommand]
    private async Task EditEvenementAsync(EvenementListItem item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"NewEventPage?idEvenement={item.IdEvenement}");
    }

    [RelayCommand]
    private async Task DeleteEvenementAsync(EvenementListItem item)
    {
        if (item == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmation de suppression",
            $"Voulez-vous vraiment supprimer l'observation de {item.Ville} ?",
            "Oui", "Non");

        if (!confirm) return;

        try
        {
            var evenement = await _context.Evenement.FindAsync(item.IdEvenement);
            if (evenement != null)
            {
                _context.Evenement.Remove(evenement);
                await _context.SaveChangesAsync();

                // Rechargement immédiat de l'UI globale
                await RefreshDataAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Impossible de supprimer l'événement : {ex.Message}", "OK");
        }
    }

    private void LoadData()
    {
        try
        {
            var stats = _context.Evenement
                .AsNoTracking()
                .Include(e => e.IdClassementNavigation)
                .GroupBy(e => e.IdClassementNavigation != null ? e.IdClassementNavigation.Nom : "Non classé")
                .Select(g => new { Classification = g.Key, Count = g.Count() })
                .ToList();

            TotalCases = stats.Sum(s => s.Count);

            Series = stats.Select(s => new PieSeries<int>
            {
                Values = new[] { s.Count },
                Name = s.Classification,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}"
            }).ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur statistiques : {ex.Message}");
        }
    }
}