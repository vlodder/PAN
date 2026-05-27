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

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<EvenementListItem> evenements = [];

        [RelayCommand]
        private async Task LoadAsync()
        {
            var items = await _evenementService.GetAllAsync();
            Evenements = new ObservableCollection<EvenementListItem>(items);
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var items = await _evenementService.SearchAsync(SearchText);
            Evenements = new ObservableCollection<EvenementListItem>(items);
        }
    }
}