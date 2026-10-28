using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class NewDonorViewModel : ObservableObject
{
    private bool _showValidation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreError))]
    public partial string NombreDonante { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TelefonoError))]
    public partial string Telefono { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DireccionError))]
    public partial string Direccion { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PesoError))]
    public partial string Peso { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TipoSangreError))]
    public partial string TipoSangre { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-25);

    [ObservableProperty]
    public partial DateTime? UltimaDonacion { get; set; }

    [ObservableProperty]
    public partial bool IsAvailable { get; set; } = true;

    public string? PhotoPath { get; set; }

    public string NombreError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(NombreDonante) ? "El nombre del donante es obligatorio." :
        NombreDonante.Trim().Length < 3 ? "El nombre debe tener al menos 3 caracteres." :
        string.Empty;

    public string TelefonoError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(Telefono) ? "El teléfono es obligatorio." :
        Telefono.Count(char.IsDigit) < 7 ? "El teléfono debe tener al menos 7 dígitos." :
        !Telefono.All(character => char.IsDigit(character) || character is '+' or ' ' or '-')
            ? "Solo se permiten números, +, espacios o guiones." :
        string.Empty;

    public string DireccionError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(Direccion) ? "La dirección es obligatoria." :
        Direccion.Trim().Length < 5 ? "Ingresa una dirección más detallada." :
        string.Empty;

    public string PesoError => _showValidation ? ValidateWeight() : string.Empty;

    public string TipoSangreError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(TipoSangre) ? "Selecciona el tipo de sangre." : string.Empty;

    [RelayCommand]
    private async Task GuardarAsync()
    {
        NotifyValidation();
        if (!FormularioValido()) return;

        int edad = CalculateAge(FechaNacimiento);
        if (edad is < 18 or > 100)
        {
            await Shell.Current.DisplayAlertAsync(
                "Fecha no válida",
                "El donante debe tener entre 18 y 100 años.",
                "Entendido");
            return;
        }

        var donante = new Donante
        {
            NombreDonante = NombreDonante.Trim(),
            TipoSangre = DonanteValidator.NormalizeBloodType(TipoSangre),
            Edad = edad,
            FechaNacimiento = FechaNacimiento,
            Telefono = Telefono.Trim(),
            Direccion = Direccion.Trim(),
            UltimaDonacion = UltimaDonacion,
            Peso = double.Parse(Peso),
            IsAvailable = IsAvailable,
            PhotoUrl = PhotoPath
        };

        string? validationError = DonanteValidator.Validate(donante);
        if (validationError is not null)
        {
            await Shell.Current.DisplayAlertAsync("Revisa los datos", validationError, "Entendido");
            return;
        }

        try
        {
            await DonanteStore.AddAsync(donante);
            LimpiarFormulario();
            await Shell.Current.DisplayAlertAsync("¡Éxito!", "Donante registrado correctamente.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("No se pudo guardar", ex.Message, "Entendido");
        }
    }

    [RelayCommand]
    private void Cancelar() => LimpiarFormulario();

    private string ValidateWeight()
    {
        if (string.IsNullOrWhiteSpace(Peso)) return "El peso es obligatorio.";
        if (!double.TryParse(Peso, out double value)) return "Ingresa un peso válido (ej: 70.5).";
        if (value < 50) return "El donante debe pesar al menos 50 kg para donar.";
        if (value > 300) return "Verifica el peso ingresado.";
        return string.Empty;
    }

    private bool FormularioValido() =>
        string.IsNullOrEmpty(NombreError) &&
        string.IsNullOrEmpty(TelefonoError) &&
        string.IsNullOrEmpty(DireccionError) &&
        string.IsNullOrEmpty(PesoError) &&
        string.IsNullOrEmpty(TipoSangreError);

    private void NotifyValidation()
    {
        _showValidation = true;
        OnPropertyChanged(nameof(NombreError));
        OnPropertyChanged(nameof(TelefonoError));
        OnPropertyChanged(nameof(DireccionError));
        OnPropertyChanged(nameof(PesoError));
        OnPropertyChanged(nameof(TipoSangreError));
    }

    private void LimpiarFormulario()
    {
        _showValidation = false;
        NombreDonante = string.Empty;
        Telefono = string.Empty;
        Direccion = string.Empty;
        Peso = string.Empty;
        TipoSangre = string.Empty;
        FechaNacimiento = DateTime.Today.AddYears(-25);
        UltimaDonacion = null;
        IsAvailable = true;
        PhotoPath = null;
    }

    private static int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;
        if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;
        return age;
    }
}
