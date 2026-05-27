using PAN.Services;
using PAN.ViewModels;

namespace PAN.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = AppService.GetRequiredService<HomeViewModel>();
        }
    }
}