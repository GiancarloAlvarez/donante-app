using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class SolicitudSangreViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombrePacienteError))]
    private string _nombrePaciente = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TipoSangreError))]
    private string _tipoSangreNecesario = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HospitalError))]
    private string _hospital = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactoError))]
    private string _numeroContacto = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CiudadError))]
    private string _ciudad = string.Empty;

    [ObservableProperty]
    private bool _esUrgente;

    [ObservableProperty]
    private DateTime _fechaSolicitud = DateTime.Today;

    public string NombrePacienteError => ValidarNombrePaciente();
    public string TipoSangreError => ValidarTipoSangre();
    public string HospitalError => ValidarHospital();
    public string ContactoError => ValidarContacto();
    public string CiudadError => ValidarCiudad();

    private string ValidarNombrePaciente() =>
        string.IsNullOrWhiteSpace(NombrePaciente) ? "El nombre del paciente es obligatorio." :
        NombrePaciente.Trim().Length < 3 ? "El nombre debe tener al menos 3 caracteres." :
        string.Empty;

    private string ValidarTipoSangre() =>
        string.IsNullOrWhiteSpace(TipoSangreNecesario) ? "Selecciona el tipo de sangre requerido." :
        string.Empty;

    private string ValidarHospital() =>
        string.IsNullOrWhiteSpace(Hospital) ? "El nombre del hospital es obligatorio." :
        Hospital.Trim().Length < 3 ? "Ingresa un nombre de hospital válido." :
        string.Empty;

    private string ValidarContacto() =>
        string.IsNullOrWhiteSpace(NumeroContacto) ? "El número de contacto es obligatorio." :
        NumeroContacto.Length < 8 ? "El número debe tener al menos 8 dígitos." :
        !NumeroContacto.All(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-')
            ? "Solo se permiten números, +, espacios o guiones." :
        string.Empty;

    private string ValidarCiudad() =>
        string.IsNullOrWhiteSpace(Ciudad) ? "La ciudad es obligatoria." :
        Ciudad.Trim().Length < 2 ? "Ingresa una ciudad válida." :
        string.Empty;

    private bool FormularioValido() =>
        string.IsNullOrEmpty(NombrePacienteError) &&
        string.IsNullOrEmpty(TipoSangreError) &&
        string.IsNullOrEmpty(HospitalError) &&
        string.IsNullOrEmpty(ContactoError) &&
        string.IsNullOrEmpty(CiudadError);

    [RelayCommand]
    private async Task GuardarAsync()
    {
        OnPropertyChanged(nameof(NombrePacienteError));
        OnPropertyChanged(nameof(TipoSangreError));
        OnPropertyChanged(nameof(HospitalError));
        OnPropertyChanged(nameof(ContactoError));
        OnPropertyChanged(nameof(CiudadError));

        if (!FormularioValido()) return;

        SolicitudSangreStore.Solicitudes.Add(new SolicitudSangre
        {
            Id = SolicitudSangreStore.Solicitudes.Count + 1,
            NombrePaciente = NombrePaciente.Trim(),
            TipoSangreNecesario = TipoSangreNecesario,
            Hospital = Hospital.Trim(),
            EsUrgente = EsUrgente,
            NumeroContacto = NumeroContacto.Trim(),
            FechaSolicitud = FechaSolicitud,
            Ciudad = Ciudad.Trim()
        });

        LimpiarFormulario();
        await Shell.Current.GoToAsync("..");
    }

    private void LimpiarFormulario()
    {
        NombrePaciente = string.Empty;
        TipoSangreNecesario = string.Empty;
        Hospital = string.Empty;
        NumeroContacto = string.Empty;
        Ciudad = string.Empty;
        EsUrgente = false;
        FechaSolicitud = DateTime.Today;
    }
}