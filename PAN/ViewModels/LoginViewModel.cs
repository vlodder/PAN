namespace PAN.ViewModels
{
    public partial class LoginViewModel(IDialogService dialogService, INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    {
        [ObservableProperty]
        private string? password;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (Password == "geipan")
            {
                await NavigationService.GoToAsync("//admin");
            }
            else
            {
                await DialogService.DisplayAlertAsync("Erreur", "Mot de passe incorrect", "OK");
            }
        }
    }
}
