using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.ViewModels;

public partial class RegistroDonacionViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DonanteIdError))]
    private string _donanteId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreDonanteError))]
    private string _nombreDonante = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CentroDonacionError))]
    private string _centroDonacion = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CiudadError))]
    private string _ciudad = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VolumenError))]
    private string _volumenMl = "450"; // default del modelo

    [ObservableProperty]
    private DateTime _fechaDonacion = DateTime.Today;

    public string DonanteIdError => ValidarDonanteId();
    public string NombreDonanteError => ValidarNombreDonante();
    public string CentroDonacionError => ValidarCentro();
    public string CiudadError => ValidarCiudad();
    public string VolumenError => ValidarVolumen();

    private string ValidarDonanteId()
    {
        if (string.IsNullOrWhiteSpace(DonanteId)) return "El ID del donante es obligatorio.";
        if (!int.TryParse(DonanteId, out int id)) return "El ID debe ser un número válido.";
        if (id <= 0) return "El ID debe ser mayor a 0.";
        var existe = DonanteStore.Donantes.Any(d => d.Id == id);
        if (!existe) return "No existe un donante con ese ID.";
        return string.Empty;
    }

    private string ValidarNombreDonante() =>
        string.IsNullOrWhiteSpace(NombreDonante) ? "El nombre del donante es obligatorio." :
        NombreDonante.Trim().Length < 3 ? "El nombre debe tener al menos 3 caracteres." :
        string.Empty;

    private string ValidarCentro() =>
        string.IsNullOrWhiteSpace(CentroDonacion) ? "El centro de donación es obligatorio." :
        CentroDonacion.Trim().Length < 3 ? "Ingresa un nombre de centro válido." :
        string.Empty;

    private string ValidarCiudad() =>
        string.IsNullOrWhiteSpace(Ciudad) ? "La ciudad es obligatoria." :
        Ciudad.Trim().Length < 2 ? "Ingresa una ciudad válida." :
        string.Empty;

    private string ValidarVolumen()
    {
        if (string.IsNullOrWhiteSpace(VolumenMl)) return "El volumen es obligatorio.";
        if (!double.TryParse(VolumenMl, out double v)) return "Ingresa un volumen válido.";
        if (v < 200) return "El volumen mínimo de donación es 200 ml.";
        if (v > 550) return "El volumen máximo de donación es 550 ml.";
        return string.Empty;
    }

    private bool FormularioValido() =>
        string.IsNullOrEmpty(DonanteIdError) &&
        string.IsNullOrEmpty(NombreDonanteError) &&
        string.IsNullOrEmpty(CentroDonacionError) &&
        string.IsNullOrEmpty(CiudadError) &&
        string.IsNullOrEmpty(VolumenError);

    [RelayCommand]
    private async Task GuardarAsync()
    {
        OnPropertyChanged(nameof(DonanteIdError));
        OnPropertyChanged(nameof(NombreDonanteError));
        OnPropertyChanged(nameof(CentroDonacionError));
        OnPropertyChanged(nameof(CiudadError));
        OnPropertyChanged(nameof(VolumenError));

        if (!FormularioValido()) return;

        var donante = DonanteStore.Donantes.FirstOrDefault(d => d.Id == int.Parse(DonanteId));
        if (donante is not null)
            donante.UltimaDonacion = FechaDonacion;

        RegistroDonacionStore.Registros.Add(new RegistroDonacion
        {
            Id = RegistroDonacionStore.Registros.Count + 1,
            DonanteId = int.Parse(DonanteId),
            NombreDonante = NombreDonante.Trim(),
            FechaDonacion = FechaDonacion,
            CentroDonacion = CentroDonacion.Trim(),
            VolumenMl = double.Parse(VolumenMl),
            Ciudad = Ciudad.Trim()
        });

        LimpiarFormulario();
        await Shell.Current.GoToAsync("..");
    }

    private void LimpiarFormulario()
    {
        DonanteId = string.Empty;
        NombreDonante = string.Empty;
        CentroDonacion = string.Empty;
        Ciudad = string.Empty;
        VolumenMl = "450";
        FechaDonacion = DateTime.Today;
    }
}