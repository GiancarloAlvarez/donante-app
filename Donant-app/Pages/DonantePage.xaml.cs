using Donant_app.Models;

namespace Donant_app.Pages;

[QueryProperty(nameof(Donante), "Donante")]
public partial class DonantePage : ContentPage
{
    private Donante? _donante;

    public Donante? Donante
    {
        get => _donante;
        set
        {
            _donante = value;
            MainThread.BeginInvokeOnMainThread(CargarDatosPerfil);
        }
    }


    public DonantePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DataReady;
        if (_donante is not null)
        {
            _donante = DonanteStore.Donantes.FirstOrDefault(donor => donor.Id == _donante.Id) ?? _donante;
        }
        CargarDatosPerfil();
    }

    private void CargarDatosPerfil()
    {
        var donante = _donante ?? DonanteStore.Donantes.FirstOrDefault();
        if (donante == null) return;
        _donante = donante;

        string iniciales = string.Concat(
            donante.NombreDonante.Split(' ')
                   .Where(p => !string.IsNullOrEmpty(p))
                   .Take(2)
                   .Select(p => p[0].ToString().ToUpper()));

        List<RegistroDonacion> history = RegistroDonacionStore.Registros
            .Where(record => record.DonanteId == donante.Id)
            .OrderByDescending(record => record.FechaDonacion)
            .ToList();
        int donationCount = history.Count;
        DateTime? lastDonation = DonorInsights.GetLastDonation(donante, history);

        LblIniciales.Text = iniciales;
        LblNombre.Text = donante.NombreDonante;
        LblTipoSangreHeader.Text = $"Tipo {donante.TipoSangre}";
        LblTipoSangre.Text = donante.TipoSangre;
        LblEdad.Text = donante.Edad.ToString();
        LblPeso.Text = donante.Peso.ToString("0");
        LblTelefono.Text = donante.Telefono;
        LblDireccion.Text = donante.Direccion;
        LblUltimaDonacion.Text = lastDonation?.ToString("dd MMM") ?? "—";
        LblTotalDonaciones.Text = donationCount.ToString();
        LblLivesSaved.Text = (donationCount * 3).ToString();
        LblDonorLevel.Text = DonorInsights.GetLevel(donationCount);

        bool hasPhoto = !string.IsNullOrWhiteSpace(donante.PhotoUrl) && File.Exists(donante.PhotoUrl);
        ProfilePhoto.IsVisible = hasPhoto;
        LblIniciales.IsVisible = !hasPhoto;
        ProfilePhoto.Source = hasPhoto ? ImageSource.FromFile(donante.PhotoUrl) : null;

        BindableLayout.SetItemsSource(RecentHistoryList, history.Take(3).ToList());
        RefreshAvailability(donante, history);
        RefreshAchievements(donationCount);
    }

    private void RefreshAvailability(Donante donor, IReadOnlyList<RegistroDonacion> history)
    {
        bool eligible = DonorInsights.IsEligible(donor, history, DateTime.Today);
        DateTime? nextEligibleDate = DonorInsights.GetNextEligibleDate(donor, history);

        string accentColor;
        string softColor;
        string title;
        string message;
        string badge;
        string icon;

        if (!donor.IsAvailable)
        {
            accentColor = "#9CA3AF";
            softColor = "#F3F4F6";
            title = "No disponible para donar";
            message = "La disponibilidad fue desactivada en el perfil";
            badge = "Inactiva";
            icon = "\uf057";
        }
        else if (eligible)
        {
            accentColor = "#10B981";
            softColor = "#ECFDF5";
            title = "Disponible para donar";
            message = "Cumple el período de descanso de 56 días";
            badge = "Disponible";
            icon = "\uf058";
        }
        else
        {
            accentColor = "#F59E0B";
            softColor = "#FFFBEB";
            title = "En período de descanso";
            message = $"Podrá donar desde el {nextEligibleDate:dd MMM yyyy}";
            badge = "En espera";
            icon = "\uf017";
        }

        Color accent = Color.FromArgb(accentColor);
        Color soft = Color.FromArgb(softColor);
        AvailabilityAccent.BackgroundColor = accent;
        AvailabilityIconBorder.BackgroundColor = soft;
        LblAvailabilityIcon.TextColor = accent;
        LblAvailabilityIcon.Text = icon;
        LblAvailabilityTitle.Text = title;
        LblAvailabilityMessage.Text = message;
        AvailabilityBadge.BackgroundColor = soft;
        LblAvailabilityBadge.TextColor = accent;
        LblAvailabilityBadge.Text = badge;
    }

    private void RefreshAchievements(int donationCount)
    {
        SetAchievement(AchievementFirstBorder, AchievementFirstIcon, AchievementFirstLabel, donationCount >= 1);
        SetAchievement(AchievementFiveBorder, AchievementFiveIcon, AchievementFiveLabel, donationCount >= 5);
        SetAchievement(AchievementTenBorder, AchievementTenIcon, AchievementTenLabel, donationCount >= 10);
        SetAchievement(AchievementTwentyBorder, AchievementTwentyIcon, AchievementTwentyLabel, donationCount >= 20);
    }

    private static void SetAchievement(Border border, Label icon, Label label, bool unlocked)
    {
        border.BackgroundColor = Color.FromArgb(unlocked ? "#FFF0F3" : "#F8F7F9");
        icon.TextColor = Color.FromArgb(unlocked ? "#DC143C" : "#D1D5DB");
        label.TextColor = Color.FromArgb(unlocked ? "#4B5563" : "#9CA3AF");
    }

    private async void OnCamaraTapped(object? sender, EventArgs e)
    {
        if (_donante is null) return;

        try
        {
            string? photoPath = await PhotoStorage.PickAndSavePhotoAsync("Selecciona una foto de perfil");
            if (photoPath is null) return;

            var updatedDonor = new Donante
            {
                Id = _donante.Id,
                NombreDonante = _donante.NombreDonante,
                TipoSangre = _donante.TipoSangre,
                Edad = _donante.Edad,
                FechaNacimiento = _donante.FechaNacimiento,
                Telefono = _donante.Telefono,
                Direccion = _donante.Direccion,
                UltimaDonacion = _donante.UltimaDonacion,
                Peso = _donante.Peso,
                IsAvailable = _donante.IsAvailable,
                PhotoUrl = photoPath,
                CreatedAt = _donante.CreatedAt
            };

            await DonanteStore.UpdateAsync(updatedDonor);
            _donante = updatedDonor;
            CargarDatosPerfil();
            await AppShell.DisplaySnackbarAsync("Foto de perfil actualizada.");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("No se pudo actualizar la foto", ex.Message, "Entendido");
        }
    }

    // ═══════════════════════════════════════
    // 1️⃣ EDITAR PERFIL → redirige a EditDonorPage
    // ═══════════════════════════════════════
    private async void OnEditarPerfilTapped(object? sender, EventArgs e)
    {
        if (_donante == null)
        {
            await AppShell.DisplaySnackbarAsync("No hay donante seleccionado.");
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(EditDonorPage),
            new Dictionary<string, object> { ["Donante"] = _donante });
    }

    
    private async void OnCerrarSesionTapped(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Estás seguro que deseas salir?",
            "Sí, salir",
            "Cancelar");

        if (!confirm)
        {
            return;
        }

        await AppShell.LogoutAsync();
    }

    private async void OnVerTodoTapped(object? sender, EventArgs e)
    {
        if (_donante is null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(HistorialDonantePage),
            new Dictionary<string, object> { ["Donante"] = _donante });
    }
}
