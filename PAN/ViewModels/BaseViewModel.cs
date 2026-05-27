namespace PAN.ViewModels
{
    public partial class BaseViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        string heading = "") : ObservableObject
    {
        public IDialogService DialogService => dialogService;

        public INavigationService NavigationService => navigationService;

        [ObservableProperty]
        public partial string Heading { get; set; } = heading;
    }
}
