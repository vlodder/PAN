using System.Collections.ObjectModel;
using System.Globalization;
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

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty] private string descriptif = string.Empty;
    [ObservableProperty] private string compteRendu = string.Empty;
    [ObservableProperty] private DateTime dateObservation = DateTime.Now;

    // Changement ici : on passe en string pour éviter les bugs de conversion MAUI liés à la virgule/point
    [ObservableProperty] private string latitude = string.Empty;
    [ObservableProperty] private string longitude = string.Empty;

    [ObservableProperty] private bool estMouvant;

    [ObservableProperty] private ObservableCollection<string> villes = new();
    [ObservableProperty] private ObservableCollection<TypeOption> types = new();
    [ObservableProperty] private ObservableCollection<Classement> classements = new();

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
            IsEditMode = true;

            if (_listsLoaded)
            {
                _ = LoadEvenementForEditAsync(value.Value);
            }
        }
        else
        {
            TitrePage = "Nouvelle observation";
            IsEditMode = false;

            Descriptif = string.Empty;
            CompteRendu = string.Empty;
            DateObservation = DateTime.Now;
            Latitude = string.Empty;
            Longitude = string.Empty;
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

                // Formatage explicite avec un point (InvariantCulture)
                Latitude = ev.Latitude.ToString(CultureInfo.InvariantCulture);
                Longitude = ev.Longitude.ToString(CultureInfo.InvariantCulture);

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

        // Parsing sécurisé des coordonnées (gère la virgule et le point)
        decimal parsedLat = 0;
        decimal parsedLon = 0;

        if (!string.IsNullOrWhiteSpace(Latitude))
            decimal.TryParse(Latitude.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedLat);

        if (!string.IsNullOrWhiteSpace(Longitude))
            decimal.TryParse(Longitude.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedLon);

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
                var ev = await _context.Evenement.FindAsync(IdEvenement.Value);
                if (ev != null)
                {
                    ev.Descriptif = Descriptif;
                    ev.DateHeureObservation = DateObservation;
                    ev.Latitude = parsedLat;
                    ev.Longitude = parsedLon;
                    ev.Estmouvant = EstMouvant;
                    ev.IdType = SelectedType.Id;

                    ev.IdClassement = SelectedClassement?.IdClassement;
                    ev.CompteRendu = string.IsNullOrWhiteSpace(CompteRendu) ? null : CompteRendu;

                    ev.IdLocalisationNavigation = loc;

                    _context.Evenement.Update(ev);
                }
            }
            else
            {
                var newEv = new Evenement
                {
                    Descriptif = Descriptif,
                    DateHeureObservation = DateObservation,
                    Latitude = parsedLat,
                    Longitude = parsedLon,
                    Estmouvant = EstMouvant,
                    IdType = SelectedType.Id,
                    IdLocalisationNavigation = loc,
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
        } }
        private async Task GoToHomeAsync()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
