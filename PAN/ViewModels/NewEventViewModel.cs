using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.context.Models;
using PAN.Services;
using Microsoft.EntityFrameworkCore;

namespace PAN.ViewModels;

[QueryProperty(nameof(IdEvenement), "idEvenement")]
public partial class NewEventViewModel : ObservableObject
{
    private readonly GeipanContext _context;
    private readonly IEvenementService _evenementService;

    [ObservableProperty]
    private int? idEvenement;

    [ObservableProperty]
    private string titrePage = "Nouvelle observation";

    // Propriété magique pour masquer/afficher les champs d'administration
    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty] private string descriptif = string.Empty;
    [ObservableProperty] private string compteRendu = string.Empty; // Nouveau champ
    [ObservableProperty] private DateTime dateObservation = DateTime.Now;
    [ObservableProperty] private decimal latitude;
    [ObservableProperty] private decimal longitude;
    [ObservableProperty] private bool estMouvant;

    // Listes alimentées par le Service et le Context
    [ObservableProperty] private ObservableCollection<string> villes = new();
    [ObservableProperty] private ObservableCollection<TypeOption> types = new();
    [ObservableProperty] private ObservableCollection<Classement> classements = new();

    // Éléments sélectionnés
    [ObservableProperty] private string selectedVille = string.Empty;
    [ObservableProperty] private TypeOption? selectedType;
    [ObservableProperty] private Classement? selectedClassement;

    private bool _listsLoaded = false;

    public NewEventViewModel(GeipanContext context, IEvenementService evenementService)
    {
        _context = context;
        _evenementService = evenementService;

        _ = LoadMasterListsAsync();
    }

    private async Task LoadMasterListsAsync()
    {
        try
        {
            var villesList = await _evenementService.GetVillesAsync();
            Villes = new ObservableCollection<string>(villesList);

            var typesList = await _evenementService.GetTypesAsync();
            Types = new ObservableCollection<TypeOption>(typesList);

            var classementsDb = await _context.Classement.ToListAsync();
            Classements = new ObservableCollection<Classement>(classementsDb);

            _listsLoaded = true;

            if (IdEvenement.HasValue && IdEvenement.Value > 0)
            {
                await LoadEvenementForEditAsync(IdEvenement.Value);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur chargement listes : {ex.Message}");
        }
    }

    partial void OnIdEvenementChanged(int? value)
    {
        if (value.HasValue && value.Value > 0)
        {
            TitrePage = "Modifier l'observation";
            IsEditMode = true; // On affiche le Cas et le Compte Rendu

            if (_listsLoaded)
            {
                _ = LoadEvenementForEditAsync(value.Value);
            }
        }
        else
        {
            TitrePage = "Nouvelle observation";
            IsEditMode = false; // On masque l'administration

            Descriptif = string.Empty;
            CompteRendu = string.Empty;
            DateObservation = DateTime.Now;
            Latitude = 0;
            Longitude = 0;
            EstMouvant = false;
            SelectedVille = string.Empty;
            SelectedType = null;
            SelectedClassement = null;
        }
    }

    private async Task LoadEvenementForEditAsync(int id)
    {
        try
        {
            var ev = await _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .FirstOrDefaultAsync(e => e.IdEvenement == id);

            if (ev != null)
            {
                Descriptif = ev.Descriptif ?? string.Empty;
                CompteRendu = ev.CompteRendu ?? string.Empty;
                DateObservation = ev.DateHeureObservation;
                Latitude = ev.Latitude;
                Longitude = ev.Longitude;
                EstMouvant = ev.Estmouvant;

                SelectedVille = ev.IdLocalisationNavigation?.Ville ?? string.Empty;
                SelectedType = Types.FirstOrDefault(t => t.Id == ev.IdType);
                SelectedClassement = Classements.FirstOrDefault(c => c.IdClassement == ev.IdClassement);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur chargement édition : {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedVille))
        {
            await Shell.Current.DisplayAlert("Champs requis", "Veuillez sélectionner une ville.", "OK");
            return;
        }

        if (SelectedType == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un type d'événement.", "OK");
            return;
        }

        try
        {
            var loc = await _context.Localisation.FirstOrDefaultAsync(l => l.Ville == SelectedVille);
            if (loc == null)
            {
                loc = new Localisation { Ville = SelectedVille };
                await _context.Localisation.AddAsync(loc);
            }

            if (IdEvenement.HasValue && IdEvenement.Value > 0)
            {
                // === MODE MODIFICATION ===
                var ev = await _context.Evenement.FindAsync(IdEvenement.Value);
                if (ev != null)
                {
                    ev.Descriptif = Descriptif;
                    ev.DateHeureObservation = DateObservation;
                    ev.Latitude = Latitude;
                    ev.Longitude = Longitude;
                    ev.Estmouvant = EstMouvant;
                    ev.IdType = SelectedType.Id;

                    // On met à jour les champs exclusifs à l'édition
                    ev.IdClassement = SelectedClassement?.IdClassement;
                    ev.CompteRendu = string.IsNullOrWhiteSpace(CompteRendu) ? null : CompteRendu;

                    ev.IdLocalisationNavigation = loc;

                    _context.Evenement.Update(ev);
                }
            }
            else
            {
                // === MODE CRÉATION ===
                var newEv = new Evenement
                {
                    Descriptif = Descriptif,
                    DateHeureObservation = DateObservation,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Estmouvant = EstMouvant,
                    IdType = SelectedType.Id,
                    IdLocalisationNavigation = loc,

                    // Forcé à null car non géré par l'utilisateur à la création
                    IdClassement = null,
                    CompteRendu = null
                };
                await _context.Evenement.AddAsync(newEv);
            }

            await _context.SaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Impossible d'enregistrer : {ex.InnerException?.Message ?? ex.Message}", "OK");
        }
    }
}