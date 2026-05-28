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
    }
}