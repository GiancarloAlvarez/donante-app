using Donant_app.Models;

namespace Donant_app.Pages;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarDatosPerfil();
    }

    private void CargarDatosPerfil()
    {
        // Si tienes un donante logueado en sesión, cárgalo aquí
        // Por ahora usa el primero del store como demo
        var donante = DonanteStore.Donantes.FirstOrDefault();
        if (donante == null) return;

        var iniciales = string.Concat(
            donante.NombreDonante.Split(' ')
                   .Where(p => !string.IsNullOrEmpty(p))
                   .Take(2)
                   .Select(p => p[0].ToString().ToUpper()));

            LblIniciales.Text = iniciales;
            LblNombre.Text = donante.NombreDonante;
            LblTipoSangreHeader.Text = $"Tipo {donante.TipoSangre}";
            LblTipoSangre.Text = donante.TipoSangre;
            LblEdad.Text = donante.Edad.ToString();
            LblPeso.Text = donante.Peso.ToString("0");
            LblTelefono.Text = donante.Telefono;
            LblDireccion.Text = donante.Direccion;
            LblUltimaDonacion.Text = donante.UltimaDonacion.HasValue
                                            ? donante.UltimaDonacion.Value.ToString("dd MMM")
                                            : "—";
        LblTotalDonaciones.Text = DonanteStore.Donantes.Count.ToString();
    }

    private async void OnCamaraTapped(object sender, EventArgs e)
    {
        var result = await MediaPicker.PickPhotoAsync();
        // Aquí puedes actualizar la foto del perfil
    }

    // ═══════════════════════════════════════
    // 1️⃣ EDITAR PERFIL → redirige a EditDonorPage
    // ═══════════════════════════════════════
    private async void OnEditarPerfilTapped(object sender, EventArgs e)
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

    
    private async void OnCerrarSesionTapped(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync(nameof(ProjectListPage));
    }

    private async void OnVerTodoTapped(object sender, EventArgs e)
    {
        await AppShell.DisplaySnackbarAsync("Historial completo próximamente.");
    }
}