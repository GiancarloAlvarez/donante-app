using Donant_app.Models;

namespace Donant_app.Pages;

[QueryProperty(nameof(Solicitud), "Solicitud")]
public partial class SolicitudDetallePage : ContentPage
{
    private SolicitudSangre? _solicitud;
    private bool _isUpdating;

    public SolicitudSangre? Solicitud
    {
        get => _solicitud;
        set
        {
            _solicitud = value;
            MainThread.BeginInvokeOnMainThread(RefreshDetails);
        }
    }

    public SolicitudDetallePage()
    {
        InitializeComponent();
    }

    private void RefreshDetails()
    {
        if (_solicitud is null)
        {
            return;
        }

        LblBloodType.Text = _solicitud.TipoSangreNecesario;
        LblPatient.Text = _solicitud.NombrePaciente;
        LblStatus.Text = _solicitud.Estado;
        LblHospital.Text = _solicitud.Hospital;
        LblCity.Text = string.IsNullOrWhiteSpace(_solicitud.Ciudad) ? "Ciudad no indicada" : _solicitud.Ciudad;
        LblDate.Text = _solicitud.FechaSolicitud.ToString("dd MMMM yyyy");
        LblContact.Text = _solicitud.NumeroContacto;
        UrgentBadge.BackgroundColor = Color.FromArgb(
            _solicitud.EstaAtendida ? "#ECFDF5" : _solicitud.EsUrgente ? "#FFF0F3" : "#FFFBEB");
        LblStatus.TextColor = Color.FromArgb(
            _solicitud.EstaAtendida ? "#059669" : _solicitud.EsUrgente ? "#DC143C" : "#D97706");
        AttendedButton.IsVisible = !_solicitud.EstaAtendida;
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnCallTapped(object? sender, TappedEventArgs e)
    {
        if (_solicitud is null || string.IsNullOrWhiteSpace(_solicitud.NumeroContacto))
        {
            await AppShell.DisplaySnackbarAsync("Esta solicitud no tiene un número de contacto.");
            return;
        }

        try
        {
            if (!PhoneDialer.Default.IsSupported)
            {
                await Clipboard.Default.SetTextAsync(_solicitud.NumeroContacto);
                await AppShell.DisplaySnackbarAsync(
                    "Las llamadas no están disponibles; el número fue copiado.");
                return;
            }

            PhoneDialer.Default.Open(_solicitud.NumeroContacto);
        }
        catch (Exception)
        {
            try
            {
                await Clipboard.Default.SetTextAsync(_solicitud.NumeroContacto);
                await AppShell.DisplaySnackbarAsync(
                    "No fue posible abrir llamadas; el número fue copiado.");
            }
            catch (Exception)
            {
                await AppShell.DisplaySnackbarAsync("No fue posible abrir la aplicación de llamadas.");
            }
        }
    }

    private async void OnCopyTapped(object? sender, TappedEventArgs e)
    {
        if (_solicitud is null || string.IsNullOrWhiteSpace(_solicitud.NumeroContacto))
        {
            await AppShell.DisplaySnackbarAsync("Esta solicitud no tiene un número de contacto.");
            return;
        }

        await Clipboard.Default.SetTextAsync(_solicitud.NumeroContacto);
        await AppShell.DisplaySnackbarAsync("Número de contacto copiado.");
    }

    private async void OnMarkAsAttendedTapped(object? sender, TappedEventArgs e)
    {
        if (_solicitud is null || _solicitud.EstaAtendida || _isUpdating)
        {
            return;
        }

        bool confirmed = await DisplayAlertAsync(
            "Completar solicitud",
            $"¿Confirmas que la solicitud de {_solicitud.NombrePaciente} ya fue atendida?",
            "Sí, completar",
            "Cancelar");
        if (!confirmed)
        {
            return;
        }

        _isUpdating = true;
        AttendedButton.IsEnabled = false;
        try
        {
            await SolicitudSangreStore.MarkAsAttendedAsync(_solicitud.Id);
            RefreshDetails();
            await AppShell.DisplaySnackbarAsync("Solicitud marcada como atendida.");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("No se pudo actualizar", ex.Message, "Entendido");
        }
        finally
        {
            _isUpdating = false;
            AttendedButton.IsEnabled = true;
        }
    }
}
