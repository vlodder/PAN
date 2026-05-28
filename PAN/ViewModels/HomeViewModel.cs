using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.Models;
using PAN.Services;
using PAN.Views;

namespace PAN.ViewModels
{
    public partial class HomeViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IEvenementService evenementService)
        : BaseViewModel(dialogService, navigationService)
    {
        private readonly IEvenementService _evenementService = evenementService;

        [ObservableProperty]
        private ObservableCollection<EvenementListItem> latestEvents = new();

        [ObservableProperty]
        private string statsText = "Chargement...";

        [RelayCommand]
        private async Task LoadAsync()
        {
            var latest = await _evenementService.GetLatestAsync(3);
            var total = await _evenementService.GetTotalCountAsync();
            var cities = await _evenementService.GetCityCountAsync();

            LatestEvents.Clear();

            foreach (var item in latest)
                LatestEvents.Add(item);

            StatsText = $"{total} observations · {cities} villes";
        }
    }
}
