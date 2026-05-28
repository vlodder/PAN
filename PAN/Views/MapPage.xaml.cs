using PAN.ViewModels;

namespace PAN.Views;

public partial class MapPage : ContentPage
{
    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (BindingContext is MapViewModel vm)
        {
            await vm.LoadMapCommand.ExecuteAsync(null);
        }
    }
}