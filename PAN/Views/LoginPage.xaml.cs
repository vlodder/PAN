namespace PAN.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Heading = "Login";
            BindingContext = viewModel;
        }
    }
}
