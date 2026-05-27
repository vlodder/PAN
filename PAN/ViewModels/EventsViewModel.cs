namespace PAN.ViewModels
{
    public partial class EventsViewModel(IDialogService dialogService, INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService, "Calendar")
    {
        [RelayCommand]
        private Task AddEventAsync() => NavigationService.GoToAsync("newevent");
    }
}
