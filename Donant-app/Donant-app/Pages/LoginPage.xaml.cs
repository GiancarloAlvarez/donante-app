namespace Donant_app.Pages;

public partial class LoginPage : ContentPage
{
    private bool _passwordVisible = false;
    public LoginPage()
	{
        //revisar por que InitializeComponent() no da error 

        InitializeComponent();
	}
    private void OnTogglePassword(object sender, EventArgs e)
    {
        _passwordVisible = !_passwordVisible;
        PasswordEntry.IsPassword = !_passwordVisible;
        EyeIcon.Text = _passwordVisible ? "\uf070" : "\uf06e"; // fa-eye-slash / fa-eye
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        // lógica de autenticación
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnForgotPassword(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Recuperar contraseña", "Se enviará un enlace a tu correo.", "OK");
    }

    private async void OnGoogleLogin(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Google", "Login con Google no implementado aún.", "OK");
    }

    private async void OnCedulaLogin(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Cédula", "Login con cédula no implementado aún.", "OK");
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HomePage));
    }



}