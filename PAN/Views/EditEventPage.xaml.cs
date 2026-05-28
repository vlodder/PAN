using PAN.ViewModels;

namespace PAN.Views;

public partial class EditEventPage : ContentPage
{
    public EditEventPage(EditEventViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
