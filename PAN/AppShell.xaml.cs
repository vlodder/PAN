namespace PAN
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = AppService.GetRequiredService<AppViewModel>();
            // Register the routes of the detail pages
            RegisterRoutes();
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("newevent", typeof(NewEventPage));
        }
    }
}
