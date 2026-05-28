using PAN.ViewModels;

namespace PAN.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage(AdminViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Déclenche l'actualisation à chaque fois que l'utilisateur arrive ou revient sur la page
        if (BindingContext is AdminViewModel viewModel)
        {
            await viewModel.RefreshDataAsync();
        }
    }
}