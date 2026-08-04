namespace Donant_app.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
        InitializeComponent();
	}

    // Solicitar sangre
    private async void OnSolicitarTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SolicitudSangrePage));
    }

    // Registrar donación
    private async void OnRegistrarDonacionTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistroDonacionPage));
    }

    // Agendar cita (próximamente)
    private async void OnAgendarCitaTapped(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync("Agendar cita próximamente.");
    }

    // Historial (próximamente)
    private async void OnHistorialTapped(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync("Historial próximamente.");
    }

}