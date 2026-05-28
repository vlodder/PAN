using PAN.Services;
using PAN.ViewModels;

namespace PAN.Views
{
    public partial class NewEventPage : ContentPage
    {
        public NewEventPage()
        {
            InitializeComponent();
            BindingContext = AppService.GetRequiredService<NewEventViewModel>();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is NewEventViewModel vm)
                await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}