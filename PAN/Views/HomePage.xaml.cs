using PAN.ViewModels;

namespace PAN.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (BindingContext is HomeViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}