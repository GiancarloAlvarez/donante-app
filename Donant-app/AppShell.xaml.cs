using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace Donant_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("registro", typeof(Pages.RegistroDonacionPage));
            Routing.RegisterRoute("solicitud", typeof(Pages.SolicitudSangrePage));
            Routing.RegisterRoute(nameof(NewDonorPage), typeof(Pages.NewDonorPage));
            Routing.RegisterRoute(nameof(DonantePage), typeof(Pages.DonantePage));
            Routing.RegisterRoute(nameof(EditDonorPage), typeof(Pages.EditDonorPage));
            Routing.RegisterRoute(nameof(NewDonorPage), typeof(Pages.NewDonorPage));
            Routing.RegisterRoute(nameof(SolicitudSangrePage), typeof(Pages.SolicitudSangrePage));
            Routing.RegisterRoute(nameof(RegistroDonacionPage), typeof(Pages.RegistroDonacionPage));
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#DC143C"),   
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task DisplayToastAsync(string message)
        {
            // Toast no funciona en Windows con MCT
            if (OperatingSystem.IsWindows())
                return;

            var toast = Toast.Make(message, textSize: 18);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }

        public static async Task LogoutAsync()
        {
            Services.InMemoryDataService.Instance.Logout();
            Preferences.Remove("remember_me");
            Preferences.Remove("saved_username");

            // Regresa al login y limpia el stack
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}