using Microsoft.Extensions.DependencyInjection;

namespace Donant_app
{
    public partial class App : Application
    {
        internal static Task DataReady { get; private set; } = Task.CompletedTask;

        public App()
        {
            InitializeComponent();
            DataReady = InitializeDataAsync();
        }

        private static async Task InitializeDataAsync()
        {
            await Task.WhenAll(
                Models.DonanteStore.InitializeAsync(),
                Models.SolicitudSangreStore.InitializeAsync(),
                Models.RegistroDonacionStore.InitializeAsync());
            await Models.DemoDataSeeder.SeedIfApplicableAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "Dona T",
                Width = 460,
                Height = 620,
                MinimumWidth = 360,
                MinimumHeight = 480
            };
        }
    }
}
