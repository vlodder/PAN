using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PAN.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    protected readonly IDialogService DialogService;
    protected readonly INavigationService NavigationService;

    public BaseViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
    {
        DialogService = dialogService;
        NavigationService = navigationService;
    }

    [RelayCommand]
    private async Task GoToHomeAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }

    [RelayCommand]
    private async Task GoToSearchAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
        await Shell.Current.GoToAsync("SearchPage");
    }

    [RelayCommand]
    private async Task GoToNewEventAsync()
    {
        await Shell.Current.GoToAsync("NewEventPage");
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("LoginPage");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
    [RelayCommand]
    private async Task GoToMapAsync()
    {
        await Shell.Current.GoToAsync("MapPage");
    }
}