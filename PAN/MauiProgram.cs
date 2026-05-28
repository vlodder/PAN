using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

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

            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
          

            builder.Services.AddSingleton<AppViewModel>();
            builder.Services.AddSingleton<EventsViewModel>();
            builder.Services.AddSingleton<EventsPage>();
            builder.Services.AddSingleton<SearchViewModel>();
            builder.Services.AddSingleton<SearchPage>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<NewEventViewModel>();
            builder.Services.AddTransient<NewEventPage>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<AdminPage>();
            builder.Services.AddScoped<IEvenementService, EvenementService>();
            builder.Services.AddScoped<PAN.context.Models.GeipanContext>();
           





            builder.Services.AddTransient<EventDetailViewModel>();
            builder.Services.AddTransient<EventDetailPage>();

            builder.Services.AddScoped<IEvenementService, EvenementService>();
            builder.Services.AddScoped<PAN.context.Models.GeipanContext>();
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<AdminPage>();

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