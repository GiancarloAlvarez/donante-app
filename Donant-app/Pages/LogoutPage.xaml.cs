namespace Donant_app.Pages;

public partial class LogoutPage : ContentPage
{
    public LogoutPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // ✅ Espera a que la página esté completamente renderizada
        Dispatcher.Dispatch(async () =>
        {
            bool confirmar = await DisplayAlert(
                "Cerrar sesión",
                "¿Estás seguro que deseas salir?",
                "Sí, salir",
                "Cancelar");

            if (confirmar)
            {
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                await Shell.Current.GoToAsync("//main");
            }
        });
    }
}