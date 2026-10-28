using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;
using System.Collections.ObjectModel;

namespace Donant_app.ViewModels;

public partial class RegistroDonacionViewModel : ObservableObject
{
    private bool _showValidation;

    public ObservableCollection<Donante> Donantes { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DonanteSeleccionadoError))]
    [NotifyPropertyChangedFor(nameof(FechaDonacionError))]
    public partial Donante? DonanteSeleccionado { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CentroDonacionError))]
    public partial string CentroDonacion { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CiudadError))]
    public partial string Ciudad { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VolumenError))]
    public partial string VolumenMl { get; set; } = "450";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FechaDonacionError))]
    public partial DateTime FechaDonacion { get; set; } = DateTime.Today;

    public string DonanteSeleccionadoError =>
        !_showValidation ? string.Empty :
        DonanteSeleccionado is null ? "Selecciona un donante." : string.Empty;

    public string CentroDonacionError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(CentroDonacion) ? "El centro de donación es obligatorio." :
        CentroDonacion.Trim().Length < 3 ? "Ingresa un nombre de centro válido." : string.Empty;

    public string CiudadError =>
        !_showValidation ? string.Empty :
        string.IsNullOrWhiteSpace(Ciudad) ? "La ciudad es obligatoria." :
        Ciudad.Trim().Length < 2 ? "Ingresa una ciudad válida." : string.Empty;

    public string VolumenError => _showValidation ? ValidateVolume() : string.Empty;

    public string FechaDonacionError => _showValidation ? ValidateDonationDate() : string.Empty;

    public async Task CargarDonantesAsync()
    {
        await App.DataReady;
        Donante? selected = DonanteSeleccionado;
        Donantes.Clear();
        foreach (Donante donor in DonanteStore.Donantes.OrderBy(donor => donor.NombreDonante))
        {
            Donantes.Add(donor);
        }

        if (selected is not null)
        {
            DonanteSeleccionado = Donantes.FirstOrDefault(donor => donor.Id == selected.Id);
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        await App.DataReady;
        NotifyValidation();
        if (!FormularioValido()) return;

        Donante? donante = DonanteSeleccionado;
        if (donante is null)
        {
            return;
        }

        var registro = new RegistroDonacion
        {
            DonanteId = donante.Id,
            NombreDonante = donante.NombreDonante,
            FechaDonacion = FechaDonacion,
            CentroDonacion = CentroDonacion.Trim(),
            VolumenMl = double.Parse(VolumenMl),
            Ciudad = Ciudad.Trim()
        };

        await RegistroDonacionStore.AddAsync(registro);
        if (donante.UltimaDonacion is null || FechaDonacion.Date > donante.UltimaDonacion.Value.Date)
        {
            donante.UltimaDonacion = FechaDonacion.Date;
            await DonanteStore.UpdateAsync(donante);
        }

        LimpiarFormulario();
        await Shell.Current.DisplayAlertAsync("Donación guardada", "El registro quedó guardado correctamente.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private string ValidateVolume()
    {
        if (string.IsNullOrWhiteSpace(VolumenMl)) return "El volumen es obligatorio.";
        if (!double.TryParse(VolumenMl, out double value)) return "Ingresa un volumen válido.";
        if (value is < 200 or > 550) return "El volumen debe estar entre 200 y 550 ml.";
        return string.Empty;
    }

    private string ValidateDonationDate()
    {
        if (FechaDonacion.Date > DateTime.Today)
        {
            return "La fecha de donación no puede estar en el futuro.";
        }

        if (DonanteSeleccionado is not null && FechaDonacion.Date < DonanteSeleccionado.FechaNacimiento.Date)
        {
            return "La fecha de donación no puede ser anterior al nacimiento del donante.";
        }

        if (DonanteSeleccionado is not null && RegistroDonacionStore.Registros.Any(record =>
                record.DonanteId == DonanteSeleccionado.Id &&
                record.FechaDonacion.Date == FechaDonacion.Date))
        {
            return "Ya existe una donación de este donante en esa fecha.";
        }

        return string.Empty;
    }

    private bool FormularioValido() =>
        string.IsNullOrEmpty(DonanteSeleccionadoError) &&
        string.IsNullOrEmpty(CentroDonacionError) &&
        string.IsNullOrEmpty(CiudadError) &&
        string.IsNullOrEmpty(VolumenError) &&
        string.IsNullOrEmpty(FechaDonacionError);

    private void NotifyValidation()
    {
        _showValidation = true;
        OnPropertyChanged(nameof(DonanteSeleccionadoError));
        OnPropertyChanged(nameof(CentroDonacionError));
        OnPropertyChanged(nameof(CiudadError));
        OnPropertyChanged(nameof(VolumenError));
        OnPropertyChanged(nameof(FechaDonacionError));
    }

    private void LimpiarFormulario()
    {
        _showValidation = false;
        DonanteSeleccionado = null;
        CentroDonacion = string.Empty;
        Ciudad = string.Empty;
        VolumenMl = "450";
        FechaDonacion = DateTime.Today;
    }
}
