namespace PAN.ViewModels
{
    public partial class EventsViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    {
        [RelayCommand]
        private Task AddEventAsync()
        {
            return Shell.Current.GoToAsync("NewEventPage");
        }
    }
}