using PAN.ViewModels;

namespace PAN.Views
{
    public partial class NewEventPage : ContentPage
    {
        private readonly NewEventViewModel _viewModel;

        public NewEventPage(NewEventViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await _viewModel.LoadCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur chargement", ex.Message, "OK");
            }
        }
    }
}