using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PAN.context.Models;
using PAN.Services;

namespace PAN.ViewModels;

[QueryProperty(nameof(IdEvenement), "idEvenement")]
public partial class NewEventViewModel(
    IDialogService dialogService,
    INavigationService navigationService,
    IEvenementService evenementService,
    GeipanContext context)
    : BaseViewModel(dialogService, navigationService)
{
    private readonly GeipanContext _context = context;
    private readonly IEvenementService _evenementService = evenementService;

    [ObservableProperty]
    private int? idEvenement;

    [ObservableProperty]
    private string titrePage = "Nouvelle observation";

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string descriptif = string.Empty;

    [ObservableProperty]
    private string compteRendu = string.Empty;

    [ObservableProperty]
    private DateTime dateObservation = DateTime.Now;

    [ObservableProperty]
    private string latitude = string.Empty;

    [ObservableProperty]
    private string longitude = string.Empty;

    [ObservableProperty]
    private bool estMouvant;

    [ObservableProperty]
    private ObservableCollection<string> villes = new();

    [ObservableProperty]
    private ObservableCollection<TypeOption> types = new();

    [ObservableProperty]
    private ObservableCollection<Classement> classements = new();

    [ObservableProperty]
    private string selectedVille = string.Empty;

    [ObservableProperty]
    private TypeOption? selectedType;

    [ObservableProperty]
    private Classement? selectedClassement;

    private bool _listsLoaded;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_listsLoaded)
            return;

        await LoadMasterListsAsync();
    }

    private async Task LoadMasterListsAsync()
    {
        try
        {
            var villesList = await _evenementService.GetVillesAsync();
            Villes = new ObservableCollection<string>(villesList);

            var typesList = await _evenementService.GetTypesAsync();
            Types = new ObservableCollection<TypeOption>(typesList);

            var classementsDb = await _context.Classement
                .AsNoTracking()
                .OrderBy(c => c.Nom)
                .ToListAsync();

            Classements = new ObservableCollection<Classement>(classementsDb);

            _listsLoaded = true;

            if (IdEvenement.HasValue && IdEvenement.Value > 0)
            {
                await LoadEvenementForEditAsync(IdEvenement.Value);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible de charger les listes : {ex.Message}",
                "OK");
        }
    }

    partial void OnIdEvenementChanged(int? value)
    {
        if (value.HasValue && value.Value > 0)
        {
            TitrePage = "Modifier l'observation";
            IsEditMode = true;

            if (_listsLoaded)
                _ = LoadEvenementForEditAsync(value.Value);
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
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEvenement == id);

            if (ev == null)
                return;

            Descriptif = ev.Descriptif ?? string.Empty;
            CompteRendu = ev.CompteRendu ?? string.Empty;
            DateObservation = ev.DateHeureObservation;
            Latitude = ev.Latitude.ToString(CultureInfo.InvariantCulture);
            Longitude = ev.Longitude.ToString(CultureInfo.InvariantCulture);
            EstMouvant = ev.Estmouvant;

            SelectedVille = ev.IdLocalisationNavigation?.Ville ?? string.Empty;
            SelectedType = Types.FirstOrDefault(t => t.Id == ev.IdType);
            SelectedClassement = Classements.FirstOrDefault(c => c.IdClassement == ev.IdClassement);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible de charger l'observation : {ex.Message}",
                "OK");
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
            await Shell.Current.DisplayAlert("Champs requis", "Veuillez sélectionner un type.", "OK");
            return;
        }

        decimal parsedLat = 0;
        decimal parsedLon = 0;

        if (!string.IsNullOrWhiteSpace(Latitude))
        {
            decimal.TryParse(
                Latitude.Replace(",", "."),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsedLat);
        }

        if (!string.IsNullOrWhiteSpace(Longitude))
        {
            decimal.TryParse(
                Longitude.Replace(",", "."),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsedLon);
        }

        try
        {
            var loc = await _context.Localisation
                .FirstOrDefaultAsync(l => l.Ville == SelectedVille);

            if (loc == null)
            {
                loc = new Localisation { Ville = SelectedVille };
                await _context.Localisation.AddAsync(loc);
                await _context.SaveChangesAsync();
            }

            if (IdEvenement.HasValue && IdEvenement.Value > 0)
            {
                var ev = await _context.Evenement
                    .FirstOrDefaultAsync(e => e.IdEvenement == IdEvenement.Value);

                if (ev == null)
                    return;

                ev.Descriptif = Descriptif;
                ev.CompteRendu = string.IsNullOrWhiteSpace(CompteRendu) ? null : CompteRendu;
                ev.DateHeureObservation = DateObservation;
                ev.Latitude = parsedLat;
                ev.Longitude = parsedLon;
                ev.Estmouvant = EstMouvant;
                ev.IdType = SelectedType.Id;
                ev.IdClassement = SelectedClassement?.IdClassement;
                ev.IdLocalisation = loc.IdLocalisation;
            }
            else
            {
                var newEv = new Evenement
                {
                    Descriptif = Descriptif,
                    CompteRendu = null,
                    DateHeureObservation = DateObservation,
                    Latitude = parsedLat,
                    Longitude = parsedLon,
                    Estmouvant = EstMouvant,
                    IdType = SelectedType.Id,
                    IdClassement = null,
                    IdLocalisation = loc.IdLocalisation
                };

                await _context.Evenement.AddAsync(newEv);
            }

            await _context.SaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erreur",
                $"Impossible d'enregistrer : {ex.InnerException?.Message ?? ex.Message}",
                "OK");
        }
    }
}