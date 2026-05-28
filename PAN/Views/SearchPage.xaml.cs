using PAN.Services;
using PAN.ViewModels;

namespace PAN.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = AppService.GetRequiredService<SearchViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is SearchViewModel vm)
            {
                await vm.InitFiltersCommand.ExecuteAsync(null);
                await vm.SearchCommand.ExecuteAsync(null);
            }
        }
    }
}