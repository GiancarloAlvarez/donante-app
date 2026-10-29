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
            Routing.RegisterRoute(nameof(NewDonorPage), typeof(Pages.NewDonorPage));
            Routing.RegisterRoute(nameof(DonantePage), typeof(Pages.DonantePage));
            Routing.RegisterRoute(nameof(EditDonorPage), typeof(Pages.EditDonorPage));
            Routing.RegisterRoute(nameof(SolicitudDetallePage), typeof(Pages.SolicitudDetallePage));
            Routing.RegisterRoute(nameof(CentrosDonacionPage), typeof(Pages.CentrosDonacionPage));
            Routing.RegisterRoute(nameof(HistorialDonantePage), typeof(Pages.HistorialDonantePage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            string destination = args.Target.Location.OriginalString;
            bool goingToLogin = destination.Contains("login", StringComparison.OrdinalIgnoreCase);
            if (goingToLogin || Services.InMemoryDataService.Instance.IsLoggedIn)
            {
                return;
            }

            args.Cancel();
            Dispatcher.Dispatch(async () => await GoToAsync("//login"));
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                await Shell.Current.DisplayAlertAsync("Dona T", message, "Entendido");
                return;
            }

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

            await Shell.Current.GoToAsync("//login");
        }

        public static Task NavigateBackOrHomeAsync()
        {
            bool hasPageToPop = Shell.Current.Navigation.NavigationStack.Count > 1;
            return Shell.Current.GoToAsync(hasPageToPop ? ".." : "//main");
        }
    }
}
