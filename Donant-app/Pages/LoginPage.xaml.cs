using Donant_app;

namespace Donant_app.Pages;

public partial class LoginPage : ContentPage
{
    private bool _passwordVisible = false;

    public LoginPage()
    {
        InitializeComponent();
    }

    private void OnTogglePassword(object sender, EventArgs e)
    {
        _passwordVisible = !_passwordVisible;

        PasswordEntry.IsPassword = !_passwordVisible;
        EyeIcon.Text = _passwordVisible ? "\uf070" : "\uf06e";
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        string usuario = UsernameEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario) ||
            string.IsNullOrWhiteSpace(password))
        {
            await AppShell.DisplaySnackbarAsync(
                "Ingresa tu correo y contraseña para continuar.");

            return;
        }

        // Usuario único de prueba para el prototipo
        const string usuarioDemo = "admin";
        const string contrasenaDemo = "admin123";

        bool credencialesCorrectas =
            string.Equals(usuario, usuarioDemo, StringComparison.OrdinalIgnoreCase) &&
            password == contrasenaDemo;

        if (!credencialesCorrectas)
        {
            await AppShell.DisplaySnackbarAsync(
                "Correo o contraseña incorrectos.");

            return;
        }

        await Shell.Current.GoToAsync("//main");
    }

    private async void OnForgotPassword(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync(
            "La recuperación de contraseña estará disponible próximamente.");
    }

    private async void OnGoogleLogin(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync(
            "El inicio de sesión con Google aún no está disponible.");
    }

    private async void OnCedulaLogin(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync(
            "El acceso mediante cédula aún no está disponible.");
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync(
            "El registro de nuevos usuarios estará disponible próximamente.");
    }
}