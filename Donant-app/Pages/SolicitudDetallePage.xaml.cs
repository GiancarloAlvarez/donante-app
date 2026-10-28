using Donant_app.Models;

namespace Donant_app.Pages;

[QueryProperty(nameof(Solicitud), "Solicitud")]
public partial class SolicitudDetallePage : ContentPage
{
    private SolicitudSangre? _solicitud;

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
        UrgentBadge.BackgroundColor = Color.FromArgb(_solicitud.EsUrgente ? "#FFF0F3" : "#FFFBEB");
        LblStatus.TextColor = Color.FromArgb(_solicitud.EsUrgente ? "#DC143C" : "#D97706");
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
                await AppShell.DisplaySnackbarAsync("Las llamadas no están disponibles en este dispositivo.");
                return;
            }

            PhoneDialer.Default.Open(_solicitud.NumeroContacto);
        }
        catch (Exception)
        {
            await AppShell.DisplaySnackbarAsync("No fue posible abrir la aplicación de llamadas.");
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
}
