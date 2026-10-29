using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class SolicitudSangreViewModel : ObservableObject
{
    private bool _showValidation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombrePacienteError))]
    public partial string NombrePaciente { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TipoSangreError))]
    public partial string TipoSangreNecesario { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HospitalError))]
    public partial string Hospital { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactoError))]
    public partial string NumeroContacto { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CiudadError))]
    public partial string Ciudad { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool EsUrgente { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FechaSolicitudError))]
    public partial DateTime FechaSolicitud { get; set; } = DateTime.Today;

    public string NombrePacienteError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(NombrePaciente) ? "El nombre del paciente es obligatorio." :
        NombrePaciente.Trim().Length < 3 ? "El nombre debe tener al menos 3 caracteres." : string.Empty;

    public string TipoSangreError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(TipoSangreNecesario) ? "Selecciona el tipo de sangre requerido." : string.Empty;

    public string HospitalError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(Hospital) ? "El nombre del hospital es obligatorio." :
        Hospital.Trim().Length < 3 ? "Ingresa un nombre de hospital válido." : string.Empty;

    public string ContactoError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(NumeroContacto) ? "El número de contacto es obligatorio." :
        NumeroContacto.Count(char.IsDigit) < 7 ? "El número debe tener al menos 7 dígitos." :
        !NumeroContacto.All(character => char.IsDigit(character) || character is '+' or ' ' or '-')
            ? "Solo se permiten números, +, espacios o guiones." : string.Empty;

    public string CiudadError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(Ciudad) ? "La ciudad es obligatoria." :
        Ciudad.Trim().Length < 2 ? "Ingresa una ciudad válida." : string.Empty;

    public string FechaSolicitudError =>
        !_showValidation ? string.Empty :
        FechaSolicitud.Date > DateTime.Today ? "La fecha de solicitud no puede estar en el futuro." : string.Empty;

    [RelayCommand]
    private async Task GuardarAsync()
    {
        await App.DataReady;
        NotifyValidation();
        if (!FormularioValido()) return;

        try
        {
            await SolicitudSangreStore.AddAsync(new SolicitudSangre
            {
                NombrePaciente = NombrePaciente.Trim(),
                TipoSangreNecesario = DonanteValidator.NormalizeBloodType(TipoSangreNecesario),
                Hospital = Hospital.Trim(),
                EsUrgente = EsUrgente,
                NumeroContacto = NumeroContacto.Trim(),
                FechaSolicitud = FechaSolicitud,
                Ciudad = Ciudad.Trim()
            });

            LimpiarFormulario();
            await Shell.Current.DisplayAlertAsync("Solicitud guardada", "La solicitud de sangre quedó registrada.", "OK");
            await AppShell.NavigateBackOrHomeAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("No se pudo guardar", ex.Message, "Entendido");
        }
    }

    [RelayCommand]
    private void Cancelar() => LimpiarFormulario();

    private bool FormularioValido() =>
        string.IsNullOrEmpty(NombrePacienteError) &&
        string.IsNullOrEmpty(TipoSangreError) &&
        string.IsNullOrEmpty(HospitalError) &&
        string.IsNullOrEmpty(ContactoError) &&
        string.IsNullOrEmpty(CiudadError) &&
        string.IsNullOrEmpty(FechaSolicitudError);

    private void NotifyValidation()
    {
        _showValidation = true;
        OnPropertyChanged(nameof(NombrePacienteError));
        OnPropertyChanged(nameof(TipoSangreError));
        OnPropertyChanged(nameof(HospitalError));
        OnPropertyChanged(nameof(ContactoError));
        OnPropertyChanged(nameof(CiudadError));
        OnPropertyChanged(nameof(FechaSolicitudError));
    }

    private void LimpiarFormulario()
    {
        _showValidation = false;
        NombrePaciente = string.Empty;
        TipoSangreNecesario = string.Empty;
        Hospital = string.Empty;
        NumeroContacto = string.Empty;
        Ciudad = string.Empty;
        EsUrgente = false;
        FechaSolicitud = DateTime.Today;
    }
}
