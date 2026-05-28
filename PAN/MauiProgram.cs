using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;

namespace PAN
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App, MainWindow, AppShell>();

            builder
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMarkup()
                .UseSkiaSharp()
                .UseLiveCharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

            // Services
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddScoped<PAN.context.Models.GeipanContext>();
            builder.Services.AddScoped<IEvenementService, EvenementService>();

            // ViewModels
            builder.Services.AddSingleton<AppViewModel>();

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<EventsViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<NewEventViewModel>();
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<EventDetailViewModel>();

            // Pages
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<EventsPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<NewEventPage>();
            builder.Services.AddTransient<AdminPage>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<EventDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(app =>
                {
                    app.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;

                        if (window.AppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                        {
                            presenter.Maximize();
                        }
                    });
                });
            });
#endif

            return builder.Build();
        }
    }
}