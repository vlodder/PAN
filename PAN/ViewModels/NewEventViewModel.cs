namespace PAN.ViewModels
{
    public partial class NewEventViewModel(IDialogService dialogService, INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    {
        [ObservableProperty]
        public partial Event Occasion { get; set; } = new();

        [RelayCommand]
        private async Task SaveAsync()
        {
            await DialogService.DisplayAlertAsync("Add Event", "Save the event details to a data store.", "OK");
            await NavigationService.GoBackAsync();
        }

        [RelayCommand]
        private Task CancelAsync() => NavigationService.GoBackAsync();
    }
}
