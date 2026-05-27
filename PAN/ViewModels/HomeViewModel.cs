using CommunityToolkit.Mvvm.Input;

namespace PAN.ViewModels
{
    public partial class HomeViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    {
        [RelayCommand]
        private Task GoToNewEventAsync() => NavigationService.GoToAsync(nameof(Views.NewEventPage));

        [RelayCommand]
        private Task GoToSearchAsync() => NavigationService.GoToAsync(nameof(Views.SearchPage));

        [RelayCommand]
        private Task GoToLoginAsync() => NavigationService.GoToAsync(nameof(Views.LoginPage));
    }
}