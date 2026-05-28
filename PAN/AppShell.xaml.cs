using PAN.Views;
using VijayAnand.MauiToolkit.Views;

namespace PAN;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(EventsPage), typeof(EventsPage));
        Routing.RegisterRoute(nameof(NewEventPage), typeof(NewEventPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
        Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
        Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));

    }
}