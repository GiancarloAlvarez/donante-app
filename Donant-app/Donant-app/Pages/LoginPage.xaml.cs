using Donant_app.Services;

namespace Donant_app.Pages
{
<<<<<<< Updated upstream
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
        
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnForgotPassword(object sender, EventArgs e)
    {
        await DisplayAlert("Recuperar contraseña", "Se enviará un enlace a tu correo.", "OK");
    }

    private async void OnGoogleLogin(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Login con Google no implementado aún.", "OK");
    }

    private async void OnCedulaLogin(object sender, EventArgs e)
    {
        await DisplayAlert("Cédula", "Login con cédula no implementado aún.", "OK");
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HomePage));
    }



=======
    public partial class LoginPage : ContentPage
    {
        private bool _passwordVisible = false;
        private readonly InMemoryDataService _svc = InMemoryDataService.Instance;

        public LoginPage()
        {
            InitializeComponent();
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            if (Preferences.Get("remember_me", false))
            {
                UsernameEntry.Text = Preferences.Get("saved_username", string.Empty);
                RememberMe.IsChecked = true;
            }
        }

        private void OnTogglePassword(object sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            PasswordEntry.IsPassword = !_passwordVisible;
            EyeIcon.Text = _passwordVisible ? "\uf070" : "\uf06e";
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim() ?? string.Empty;
            string password = PasswordEntry.Text?.Trim() ?? string.Empty;

            // ── Validación de campos vacíos ──────────────────────────────
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Campos requeridos",
                    "Ingresa tu usuario y contraseña para continuar.", "Entendido");
                return;
            }

            // ── Autenticación contra credenciales fijas ──────────────────
            bool ok = _svc.Login(username, password);

            if (!ok)
            {
                await DisplayAlert("Acceso denegado",
                    "Usuario o contraseña incorrectos.\n\nCredenciales válidas:\n• admin / admin123\n• donante / dona123",
                    "Intentar de nuevo");
                PasswordEntry.Text = string.Empty;
                return;
            }

            // ── Recordar sesión ──────────────────────────────────────────
            if (RememberMe.IsChecked)
            {
                Preferences.Set("remember_me", true);
                Preferences.Set("saved_username", username);
            }
            else
            {
                Preferences.Remove("remember_me");
                Preferences.Remove("saved_username");
            }


            try
            {
                // tu lógica actual aquí
                await Shell.Current.GoToAsync("//main");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message + "\n\n" + ex.StackTrace, "OK");
            }



            // ── Navegar al app principal ─────────────────────────────────
            await Shell.Current.GoToAsync("//main");
        }

        private async void OnForgotPassword(object sender, EventArgs e)
        {
            await DisplayAlert("Recuperar contraseña",
                "Contacta al administrador del sistema.", "OK");
        }

        private async void OnGoogleLogin(object sender, EventArgs e) =>
            await DisplayAlert("Google Sign-In", "No disponible en esta versión.", "OK");

        private async void OnCedulaLogin(object sender, EventArgs e) =>
            await DisplayAlert("Login con Cédula", "No disponible en esta versión.", "OK");

        private async void OnRegisterTapped(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("//MainTabs/NewDonorPage");
    }
>>>>>>> Stashed changes
}