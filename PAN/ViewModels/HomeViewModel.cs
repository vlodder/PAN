using CommunityToolkit.Mvvm.Input;
using PAN.Services;
using PAN.Views;

namespace PAN.ViewModels
{
    public partial class HomeViewModel(
        IDialogService dialogService,
        INavigationService navigationService)
        : BaseViewModel(dialogService, navigationService)
    { }
}
       
