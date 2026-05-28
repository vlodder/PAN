using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.Services;
using System.Collections.ObjectModel;

namespace PAN.ViewModels
{
    public partial class NewEventViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    {
        [ObservableProperty]
        private DateTime dateObservation = DateTime.Now;

        [ObservableProperty]
        private string descriptif = string.Empty;

        [ObservableProperty]
        private string ville = string.Empty;

        [ObservableProperty]
        private bool estMouvant;

        [ObservableProperty]
        private string typeSelectionne;

        [ObservableProperty]
        private ObservableCollection<string> types =
        [
            "Point lumineux",
            "Tâche lumineuse",
            "Objet opaque",
            "Autre"
        ];

        [RelayCommand]
        private async Task SaveAsync()
        {
            await Shell.Current.DisplayAlert("Info", "Enregistrement à faire", "OK");
        }
        private async Task GoToHomeAsync()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}