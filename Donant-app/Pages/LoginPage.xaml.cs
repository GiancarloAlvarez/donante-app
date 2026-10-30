namespace Donant_app.Pages;

public partial class LoginPage : ContentPage
{
    private bool _passwordVisible;

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        bool rememberMe = Preferences.Default.Get("remember_me", false);
        RememberMe.IsChecked = rememberMe;
        UsernameEntry.Text = rememberMe
            ? Preferences.Default.Get("saved_username", string.Empty)
            : string.Empty;
        PasswordEntry.Text = string.Empty;
    }

    private void OnTogglePassword(object? sender, EventArgs e)
    {
        _passwordVisible = !_passwordVisible;
        PasswordEntry.IsPassword = !_passwordVisible;
        EyeIcon.Text = _passwordVisible ? "\uf070" : "\uf06e";
    }

    private async void OnLoginTapped(object? sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        if (username.Length < 3 || password.Length < 6)
        {
            await AppShell.DisplaySnackbarAsync("Ingresa un usuario y una contraseña válidos.");
            return;
        }

        if (!Services.InMemoryDataService.Instance.Login(username, password))
        {
            PasswordEntry.Text = string.Empty;
            await AppShell.DisplaySnackbarAsync("Usuario o contraseña incorrectos.");
            return;
        }

        Preferences.Default.Set("remember_me", RememberMe.IsChecked);
        if (RememberMe.IsChecked)
        {
            Preferences.Default.Set("saved_username", username);
        }
        else
        {
            Preferences.Default.Remove("saved_username");
        }

        await App.DataReady;
        await Shell.Current.GoToAsync("//main");
    }

}
