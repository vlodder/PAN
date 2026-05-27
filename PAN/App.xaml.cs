using System.Reflection;

namespace PAN
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = PlatformAppTheme;
        }

        public static string MauiVersion
        {
            get
            {
                return field ??= GetVersion();

                static string GetVersion()
                {
                    var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
                    return $".NET MAUI ver. {version[..version.IndexOf('+')]}";
                }
            }
        }
    }
}
