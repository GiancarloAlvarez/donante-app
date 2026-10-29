using Donant_app.Models;

namespace Donant_app.Pages;

public partial class HomePage : ContentPage
{
    private SolicitudSangre? _urgentRequest;

    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DataReady;
        RefreshDashboard();
    }

    private void RefreshDashboard()
    {
        string username = Services.InMemoryDataService.Instance.CurrentUser?.Username ?? "Dona T";
        LblUserName.Text = username;
        LblInitials.Text = string.Concat(username.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));

        Donante? donor = DonanteStore.Donantes.FirstOrDefault();
        if (donor is not null)
        {
            LblBloodType.Text = $"{donor.TipoSangre} · {donor.NombreDonante}";
            DateTime nextDonation = (donor.UltimaDonacion ?? DateTime.Today.AddDays(-56)).Date.AddDays(56);
            LblNextDonation.Text = nextDonation <= DateTime.Today
                ? "Disponible para donar ahora"
                : $"Próxima donación disponible: {nextDonation:dd MMM}";
        }
        else
        {
            LblBloodType.Text = "Sin donante asociado";
            LblNextDonation.Text = "Registra un donante para comenzar";
        }

        int donationCount = RegistroDonacionStore.Registros.Count;
        LblDonationCount.Text = donationCount.ToString();
        LblLivesCount.Text = (donationCount * 3).ToString();
        LblOverallLevel.Text = DonorInsights.GetLevel(donationCount).Replace("Donante ", string.Empty);
        BindableLayout.SetItemsSource(FeaturedDonorsList, DonanteStore.Donantes.Take(3).ToList());

        _urgentRequest = SolicitudSangreStore.Solicitudes
            .Where(request => request.EsUrgente && !request.EstaAtendida)
            .OrderByDescending(request => request.FechaSolicitud)
            .FirstOrDefault();

        UrgentCard.IsVisible = _urgentRequest is not null;
        if (_urgentRequest is not null)
        {
            LblUrgentTitle.Text = $"Urgente: se necesita {_urgentRequest.TipoSangreNecesario}";
            LblUrgentSummary.Text = $"{_urgentRequest.Hospital} · {_urgentRequest.Ciudad}";
        }
    }

    private async void OnRegisterDonationTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//registro");

    private async void OnRequestBloodTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//solicitud");

    private async void OnCentersTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(CentrosDonacionPage));

    private async void OnHistoryTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//projects?section=history");

    private async void OnViewAllDonorsTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//projects?section=donors");

    private async void OnFeaturedDonorTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not Donante donor)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(DonantePage),
            new Dictionary<string, object> { ["Donante"] = donor });
    }

    private async void OnHelpRequestTapped(object? sender, TappedEventArgs e)
    {
        if (_urgentRequest is null)
        {
            await AppShell.DisplaySnackbarAsync("No hay solicitudes urgentes activas.");
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(SolicitudDetallePage),
            new Dictionary<string, object> { ["Solicitud"] = _urgentRequest });
    }

    private async void OnNotificationsTapped(object? sender, TappedEventArgs e)
    {
        List<SolicitudSangre> urgentRequests = SolicitudSangreStore.Solicitudes
            .Where(request => request.EsUrgente && !request.EstaAtendida)
            .OrderByDescending(request => request.FechaSolicitud)
            .ToList();

        if (urgentRequests.Count == 0)
        {
            await DisplayAlertAsync("Notificaciones", "No hay solicitudes urgentes activas.", "Entendido");
            return;
        }

        string summary = string.Join(
            Environment.NewLine,
            urgentRequests.Take(3).Select(request =>
                $"• {request.TipoSangreNecesario}: {request.Hospital} ({request.Ciudad})"));
        if (urgentRequests.Count > 3)
        {
            summary += $"{Environment.NewLine}• y {urgentRequests.Count - 3} más";
        }

        bool viewRequests = await DisplayAlertAsync(
            $"Solicitudes urgentes ({urgentRequests.Count})",
            summary,
            "Ver solicitudes",
            "Cerrar");

        if (viewRequests)
        {
            await Shell.Current.GoToAsync("//projects?section=requests");
        }
    }
}
