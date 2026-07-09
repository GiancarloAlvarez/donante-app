using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UsernameError))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordError))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _recordarSesion;

    [ObservableProperty]
    private bool _isPasswordVisible;

    public string UsernameError => ValidarUsername();
    public string PasswordError => ValidarPassword();

    private string ValidarUsername() =>
        string.IsNullOrWhiteSpace(Username) ? "El usuario es obligatorio." :
        Username.Trim().Length < 3 ? "El usuario debe tener al menos 3 caracteres." :
        string.Empty;

    private string ValidarPassword() =>
        string.IsNullOrWhiteSpace(Password) ? "La contraseña es obligatoria." :
        Password.Length < 6 ? "La contraseña debe tener al menos 6 caracteres." :
        string.Empty;

    private bool FormularioValido() =>
        string.IsNullOrEmpty(UsernameError) &&
        string.IsNullOrEmpty(PasswordError);

    [RelayCommand]
    private void TogglePassword() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private async Task LoginAsync()
    {
        OnPropertyChanged(nameof(UsernameError));
        OnPropertyChanged(nameof(PasswordError));

        if (!FormularioValido()) return;

        var usuarioValido = Username.Trim() == "admin" && Password == "admin123";

        if (!usuarioValido)
        {
            OnPropertyChanged(nameof(PasswordError));
            _passwordOverride = "Usuario o contraseña incorrectos.";
            OnPropertyChanged(nameof(PasswordError));
            return;
        }

        _passwordOverride = null;
        Application.Current!.MainPage = new AppShell();
    }

    private string? _passwordOverride;
    private string ValidarPasswordConOverride() =>
        _passwordOverride ?? ValidarPassword();
}