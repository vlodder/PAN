using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.context.Models;
using Microsoft.EntityFrameworkCore;

namespace PAN.ViewModels;

[QueryProperty(nameof(IdEvenement), "idEvenement")]
public partial class NewEventViewModel : ObservableObject
{
    private readonly GeipanContext _context;

    [ObservableProperty]
    private int? idEvenement;

    [ObservableProperty]
    private string titrePage = "Nouvelle observation";

    [ObservableProperty] private string descriptif;
    [ObservableProperty] private DateTime dateObservation = DateTime.Now;
    [ObservableProperty] private decimal latitude;
    [ObservableProperty] private decimal longitude;
    [ObservableProperty] private bool estMouvant;

    // Collections pour alimenter les listes déroulantes (Pickers)
    [ObservableProperty] private ObservableCollection<PAN.context.Models.Type> types = new();
    [ObservableProperty] private ObservableCollection<Classement> classements = new();
    [ObservableProperty] private ObservableCollection<Localisation> localisations = new();

    // Éléments actuellement sélectionnés dans l'interface
    [ObservableProperty] private PAN.context.Models.Type selectedType;
    [ObservableProperty] private Classement selectedClassement;
    [ObservableProperty] private Localisation selectedLocalisation;

    public NewEventViewModel(GeipanContext context)
    {
        _context = context;
    }

    partial void OnIdEvenementChanged(int? value)
    {
        _ = LoadPageDataAsync(value);
    }

    private async Task LoadPageDataAsync(int? id)
    {
        try
        {
            // 1. Chargement synchrone de tous les référentiels depuis la base SQLite
            var typesDb = await _context.Type.ToListAsync();
            Types = new ObservableCollection<PAN.context.Models.Type>(typesDb);

            var classementsDb = await _context.Classement.ToListAsync();
            Classements = new ObservableCollection<Classement>(classementsDb);

            var localisationsDb = await _context.Localisation.OrderBy(l => l.Ville).ToListAsync();
            Localisations = new ObservableCollection<Localisation>(localisationsDb);

            // 2. Initialisation du formulaire selon le contexte (Création ou Édition)
            if (id.HasValue && id.Value > 0)
            {
                TitrePage = "Modifier l'observation";
                await LoadEvenementForEditAsync(id.Value);
            }
            else
            {
                TitrePage = "Nouvelle observation";
                ResetFields();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur d'initialisation de la page : {ex.Message}");
        }
    }

    private void ResetFields()
    {
        Descriptif = string.Empty;
        DateObservation = DateTime.Now;
        Latitude = 0;
        Longitude = 0;
        EstMouvant = false;
        SelectedType = Types.FirstOrDefault();
        SelectedClassement = null;
        SelectedLocalisation = null;
    }

    private async Task LoadEvenementForEditAsync(int id)
    {
        var ev = await _context.Evenement
            .Include(e => e.IdLocalisationNavigation)
            .FirstOrDefaultAsync(e => e.IdEvenement == id);

        if (ev != null)
        {
            Descriptif = ev.Descriptif;
            DateObservation = ev.DateHeureObservation;
            Latitude = ev.Latitude;
            Longitude = ev.Longitude;
            EstMouvant = ev.Estmouvant;

            // Sélection des objets correspondants en comparant les identifiants uniques
            SelectedType = Types.FirstOrDefault(t => t.IdType == ev.IdType);
            SelectedClassement = Classements.FirstOrDefault(c => c.IdClassement == ev.IdClassement);
            SelectedLocalisation = Localisations.FirstOrDefault(l => l.IdLocalisation == ev.IdLocalisation);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedLocalisation == null)
        {
            await Shell.Current.DisplayAlert("Champs requis", "Veuillez sélectionner une ville dans la liste.", "OK");
            return;
        }

        if (SelectedType == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un type d'événement.", "OK");
            return;
        }

        try
        {
            if (IdEvenement.HasValue && IdEvenement.Value > 0)
            {
                // Mode Modification
                var ev = await _context.Evenement.FindAsync(IdEvenement.Value);

                if (ev != null)
                {
                    ev.Descriptif = Descriptif;
                    ev.DateHeureObservation = DateObservation;
                    ev.Latitude = Latitude;
                    ev.Longitude = Longitude;
                    ev.Estmouvant = EstMouvant;
                    ev.IdType = SelectedType.IdType;
                    ev.IdClassement = SelectedClassement?.IdClassement;
                    ev.IdLocalisation = SelectedLocalisation.IdLocalisation; // Clé étrangère mise à jour directement

                    _context.Evenement.Update(ev);
                }
            }
            else
            {
                // Mode Création
                var newEv = new Evenement
                {
                    Descriptif = Descriptif,
                    DateHeureObservation = DateObservation,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Estmouvant = EstMouvant,
                    IdType = SelectedType.IdType,
                    IdClassement = SelectedClassement?.IdClassement,
                    IdLocalisation = SelectedLocalisation.IdLocalisation
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