namespace Donant_app.Pages;

public partial class LogoutPage : ContentPage
{
    private bool _promptVisible;

    public LogoutPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_promptVisible) return;

        _promptVisible = true;
        Dispatcher.Dispatch(PromptForLogoutAsync);
    }

    private async void PromptForLogoutAsync()
    {
        bool confirm = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Estás seguro que deseas salir?",
            "Sí, salir",
            "Cancelar");

        if (confirm)
        {
            await AppShell.LogoutAsync();
        }
        else
        {
            await Shell.Current.GoToAsync("//main");
        }

        _promptVisible = false;
    }
}
