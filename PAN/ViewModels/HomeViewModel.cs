using CommunityToolkit.Mvvm.Input;

namespace PAN.ViewModels
{
    public partial class HomeViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    { }
    

}