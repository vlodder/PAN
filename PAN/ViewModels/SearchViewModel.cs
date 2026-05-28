using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.Models;
using PAN.Services;

namespace PAN.ViewModels
{
    public partial class SearchViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IEvenementService evenementService)
        : BaseViewModel(dialogService, navigationService)
    {
        private readonly IEvenementService _evenementService = evenementService;

        private const int PageSize = 20;
        private int _currentSkip = 0;
        private bool _isLoading = false;
        private bool _hasMoreItems = true;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<EvenementListItem> evenements = [];

        [RelayCommand]
        private async Task LoadAsync()
        {
            var items = await _evenementService.GetAllAsync();
            Evenements = new ObservableCollection<EvenementListItem>(items);
        private string selectedVille = string.Empty;

        [ObservableProperty]
        private TypeOption? selectedType;

        [ObservableProperty]
        private string selectedMouvement = "Tous";

        [ObservableProperty]
        private ObservableCollection<string> villes = new();

        [ObservableProperty]
        private ObservableCollection<TypeOption> types = new();

        [ObservableProperty]
        private ObservableCollection<string> mouvements = new()
        {
            "Tous",
            "Oui",
            "Non"
        };

        [ObservableProperty]
        private ObservableCollection<EvenementListItem> evenements = new();

        [RelayCommand]
        private async Task InitFiltersAsync()
        {
            try
            {
                var villesList = await _evenementService.GetVillesAsync();
                Villes = new ObservableCollection<string>(villesList);

                var typesList = await _evenementService.GetTypesAsync();
                Types = new ObservableCollection<TypeOption>(typesList);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Erreur",
                    $"Impossible de charger les filtres : {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var items = await _evenementService.SearchAsync(SearchText);
            Evenements = new ObservableCollection<EvenementListItem>(items);
            _currentSkip = 0;
            _hasMoreItems = true;
            Evenements.Clear();

            await LoadMoreAsync();
        }

        [RelayCommand]
        private async Task LoadMoreAsync()
        {
            if (_isLoading || !_hasMoreItems)
                return;

            try
            {
                _isLoading = true;

                bool? estMouvant = SelectedMouvement switch
                {
                    "Oui" => true,
                    "Non" => false,
                    _ => null
                };

                var items = await _evenementService.SearchPagedAsync(
                    SearchText,
                    SelectedVille,
                    SelectedType?.Id,
                    estMouvant,
                    _currentSkip,
                    PageSize);

                foreach (var item in items)
                {
                    Evenements.Add(item);
                }

                _currentSkip += items.Count;
                _hasMoreItems = items.Count == PageSize;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Erreur",
                    $"Impossible de charger les événements : {ex.Message}",
                    "OK");
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        private async Task OpenDetailAsync(EvenementListItem item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"EventDetailPage?idEvenement={item.IdEvenement}");
        }
    }
}