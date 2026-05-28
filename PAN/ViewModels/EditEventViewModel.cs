using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.context.Models;
using PAN.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace PAN.ViewModels;

[QueryProperty(nameof(EventId), "id")]
public partial class EditEventViewModel : BaseViewModel
{
    private readonly GeipanContext _context;
    private int _currentEventId;

    [ObservableProperty]
    private int eventId;

    [ObservableProperty]
    private Evenement? evenement;

    [ObservableProperty]
    private ObservableCollection<Classement> classements = [];

    [ObservableProperty]
    private ObservableCollection<context.Models.Type> types = [];

    [ObservableProperty]
    private ObservableCollection<Phenomene> phenomenes = [];

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isNotLoading = true;

    public EditEventViewModel(GeipanContext context, 
                              IDialogService dialogService,
                              INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _context = context;
    }

    partial void OnEventIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadEventInternal(value);
        }
    }

    [RelayCommand]
    public async Task LoadEvent(int idEvenement)
    {
        await LoadEventInternal(idEvenement);
    }

    private async Task LoadEventInternal(int idEvenement)
    {
        try
        {
            IsLoading = true;
            _currentEventId = idEvenement;

            System.Diagnostics.Debug.WriteLine($"Loading event with ID: {idEvenement}");

            // Charger l'événement avec ses relations
            var evenement = await _context.Evenement
                .Include(e => e.IdClassementNavigation)
                .Include(e => e.IdTypeNavigation)
                .Include(e => e.IdPhenomeneNavigation)
                .Include(e => e.IdLocalisationNavigation)
                .FirstOrDefaultAsync(e => e.IdEvenement == idEvenement);

            if (evenement != null)
            {
                Evenement = evenement;
                System.Diagnostics.Debug.WriteLine($"Event loaded: {evenement.IdEvenement}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Event not found with ID: {idEvenement}");
                await DialogService.DisplayAlertAsync("Erreur", "Événement non trouvé", "OK");
                await NavigationService.GoBackAsync();
                return;
            }

            // Charger les listes de lookup
            var classementsList = await _context.Classement.OrderBy(c => c.Nom).ToListAsync();
            Classements = new ObservableCollection<Classement>(classementsList);

            var typesList = await _context.Type.OrderBy(t => t.Nom).ToListAsync();
            Types = new ObservableCollection<context.Models.Type>(typesList);

            var phenomenesList = await _context.Phenomene.OrderBy(p => p.Nom).ToListAsync();
            Phenomenes = new ObservableCollection<Phenomene>(phenomenesList);

            System.Diagnostics.Debug.WriteLine($"Lookups loaded: {Classements.Count} classements, {Types.Count} types, {Phenomenes.Count} phénomènes");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading event: {ex.Message}\n{ex.StackTrace}");
            await DialogService.DisplayAlertAsync("Erreur", $"Erreur lors du chargement: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Evenement == null)
            return;

        try
        {
            IsLoading = true;

            System.Diagnostics.Debug.WriteLine($"Saving event: {Evenement.IdEvenement}");

            // Mettre à jour l'événement en base de données
            _context.Evenement.Update(Evenement);
            await _context.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine($"Event saved successfully");

            await DialogService.DisplayAlertAsync("Succès", "Événement modifié avec succès", "OK");
            await NavigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}\n{ex.StackTrace}");
            await DialogService.DisplayAlertAsync("Erreur", $"Erreur lors de la sauvegarde: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        // On vérifie s'il y a une modale ouverte et on la dépile explicitement
        if (Shell.Current.Navigation.ModalStack.Count > 0)
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
        else
        {
            // Fallback sur ton service
            await NavigationService.GoBackAsync();
        }
    }
}

