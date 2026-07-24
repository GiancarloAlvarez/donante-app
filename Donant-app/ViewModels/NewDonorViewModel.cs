using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class NewDonorViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreError))]
    private string _nombreDonante = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TelefonoError))]
    private string _telefono = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DireccionError))]
    private string _direccion = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PesoError))]
    private string _peso = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TipoSangreError))]
    private string _tipoSangre = string.Empty;

    [ObservableProperty]
    private DateTime _fechaNacimiento = DateTime.Today.AddYears(-25);

    [ObservableProperty]
    private DateTime? _ultimaDonacion = null;

    [ObservableProperty]
    private bool _isAvailable = true;

    public string NombreError => ValidarNombre();
    public string TelefonoError => ValidarTelefono();
    public string DireccionError => ValidarDireccion();
    public string PesoError => ValidarPeso();
    public string TipoSangreError => ValidarTipoSangre();

    private string ValidarNombre() =>
        string.IsNullOrWhiteSpace(NombreDonante) ? "El nombre del donante es obligatorio." :
        NombreDonante.Trim().Length < 3 ? "El nombre debe tener al menos 3 caracteres." :
        string.Empty;

    private string ValidarTelefono() =>
        string.IsNullOrWhiteSpace(Telefono) ? "El teléfono es obligatorio." :
        Telefono.Length < 8 ? "El teléfono debe tener al menos 8 dígitos." :
        !Telefono.All(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-')
            ? "Solo se permiten números, +, espacios o guiones." :
        string.Empty;

    private string ValidarDireccion() =>
        string.IsNullOrWhiteSpace(Direccion) ? "La dirección es obligatoria." :
        Direccion.Trim().Length < 5 ? "Ingresa una dirección más detallada." :
        string.Empty;

    private string ValidarPeso()
    {
        if (string.IsNullOrWhiteSpace(Peso)) return "El peso es obligatorio.";
        if (!double.TryParse(Peso, out double p)) return "Ingresa un peso válido (ej: 70.5).";
        if (p < 50) return "El donante debe pesar al menos 50 kg para donar.";
        if (p > 200) return "Verifica el peso ingresado.";
        return string.Empty;
    }

    private string ValidarTipoSangre() =>
        string.IsNullOrWhiteSpace(_tipoSangre) ? "Selecciona el tipo de sangre." :
        string.Empty;

    private bool FormularioValido() =>
        string.IsNullOrEmpty(NombreError) &&
        string.IsNullOrEmpty(TelefonoError) &&
        string.IsNullOrEmpty(DireccionError) &&
        string.IsNullOrEmpty(PesoError) &&
        string.IsNullOrEmpty(TipoSangreError);

    [RelayCommand]
    private async Task GuardarAsync()
    {
        OnPropertyChanged(nameof(NombreError));
        OnPropertyChanged(nameof(TelefonoError));
        OnPropertyChanged(nameof(DireccionError));
        OnPropertyChanged(nameof(PesoError));
        OnPropertyChanged(nameof(TipoSangreError));

        if (!FormularioValido()) return;

        var edad = DateTime.Today.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

        DonanteStore.Donantes.Add(new Donante
        {
            Id = DonanteStore.Donantes.Count + 1,
            NombreDonante = NombreDonante.Trim(),
            TipoSangre = TipoSangre,
            Edad = edad,
            FechaNacimiento = FechaNacimiento,
            Telefono = Telefono.Trim(),
            Direccion = Direccion.Trim(),
            UltimaDonacion = UltimaDonacion,
            Peso = double.Parse(Peso),
            IsAvailable = IsAvailable,
            CreatedAt = DateTime.Now
        });
        LimpiarFormulario();
        await Shell.Current.DisplayAlertAsync("¡Éxito!", "Donante registrado correctamente.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private void LimpiarFormulario()
    {
        NombreDonante = string.Empty;
        Telefono = string.Empty;
        Direccion = string.Empty;
        Peso = string.Empty;
        TipoSangre = string.Empty;
        FechaNacimiento = DateTime.Today.AddYears(-25);
        UltimaDonacion = null;
        IsAvailable = true;
    }

    [RelayCommand]
    private void Cancelar()
    {
        LimpiarFormulario();
    }
}