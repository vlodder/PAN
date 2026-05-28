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
                await Shell.Current.GoToAsync("AdminPage");
            }
            else
            {
                await DialogService.DisplayAlertAsync("Erreur", "Mot de passe incorrect", "OK");
            }
        }


        private async Task GoToHomeAsync()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}
