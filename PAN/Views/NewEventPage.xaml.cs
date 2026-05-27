namespace PAN.Views
{
    public partial class NewEventPage : ContentPage
    {
        public NewEventPage(NewEventViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Heading = "New Event";
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed() => false;
    }
}
