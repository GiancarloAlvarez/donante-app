using Donant_app.Models;


namespace Donant_app.Pages;

[QueryProperty(nameof(Donante), "Donante")]
public partial class EditDonorPage : ContentPage
{
    private Donante? _donante;
    private string? _fotoPath = null;

    public Donante? Donante
    {
        get => _donante;
        set
        {
            _donante = value;
            // OnAppearing puede dispararse antes que el QueryProperty,
            // por eso también precargamos aquí
            MainThread.BeginInvokeOnMainThread(PrecargarCampos);
        }
    }

    public EditDonorPage()
    {
        InitializeComponent();
    }

    

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PrecargarCampos();
    }

    // ── Precarga todos los campos con los datos del donante recibido ──
    private void PrecargarCampos()
    {
        if (_donante == null) return;

        EntryNombre.Text = _donante.NombreDonante;
        EntryTelefono.Text = _donante.Telefono;
        EntryDireccion.Text = _donante.Direccion;
        EntryPeso.Text = _donante.Peso > 0 ? _donante.Peso.ToString("0") : string.Empty;

        // Tipo de sangre
        PickerSangre.SelectedItem = _donante.TipoSangre;

        // Fecha de nacimiento
        if (_donante.FechaNacimiento != default)
        {
            DatePickerNacimiento.Date = _donante.FechaNacimiento;
            LabelFechaNacimiento.Text = _donante.FechaNacimiento.ToString("dd/MM/yyyy");
            LabelFechaNacimiento.TextColor = Color.FromArgb("#1F2937");
        }

        // Última donación
        if (_donante.UltimaDonacion.HasValue)
            DateUltimaDonacion.Date = _donante.UltimaDonacion.Value;

        // Disponibilidad
        SwitchDisponible.IsToggled = _donante.IsAvailable;

        // Foto
        if (!string.IsNullOrEmpty(_donante.PhotoUrl))
        {
            ImgFoto.Source = ImageSource.FromFile(_donante.PhotoUrl);
            ImgFoto.IsVisible = true;
            LblIconoFoto.IsVisible = false;
        }
    }

    // ── DatePicker de nacimiento ──
    private void OnFechaNacimientoSelected(object sender, DateChangedEventArgs e)
    {
        LabelFechaNacimiento.Text = string.Format("{0:dd/MM/yyyy}", e.NewDate);
        LabelFechaNacimiento.TextColor = Color.FromArgb("#1F2937");
    }

    // ── Foto ──
    private async void OnFotoTapped(object sender, EventArgs e)
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result == null) return;

        _fotoPath = result.FullPath;
        ImgFoto.Source = ImageSource.FromFile(_fotoPath);
        ImgFoto.IsVisible = true;
        LblIconoFoto.IsVisible = false;
    }

    // ── Validación ──
    private bool Validar()
    {
        bool ok = true;

        LblNombreError.IsVisible = false;
        LblSangreError.IsVisible = false;
        LblTelefonoError.IsVisible = false;
        LblDireccionError.IsVisible = false;
        LblPesoError.IsVisible = false;

        if (string.IsNullOrWhiteSpace(EntryNombre.Text))
        {
            LblNombreError.Text = "El nombre es obligatorio.";
            LblNombreError.IsVisible = true;
            ok = false;
        }

        if (PickerSangre.SelectedItem == null)
        {
            LblSangreError.Text = "Selecciona el tipo de sangre.";
            LblSangreError.IsVisible = true;
            ok = false;
        }

        if (string.IsNullOrWhiteSpace(EntryTelefono.Text))
        {
            LblTelefonoError.Text = "El teléfono es obligatorio.";
            LblTelefonoError.IsVisible = true;
            ok = false;
        }

        if (string.IsNullOrWhiteSpace(EntryDireccion.Text))
        {
            LblDireccionError.Text = "La dirección es obligatoria.";
            LblDireccionError.IsVisible = true;
            ok = false;
        }

        if (!double.TryParse(EntryPeso.Text, out double peso) || peso < 50)
        {
            LblPesoError.Text = "Ingresa un peso válido (mín. 50 kg).";
            LblPesoError.IsVisible = true;
            ok = false;
        }

        return ok;
    }

    // ── Guardar cambios ──
    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        if (_donante == null) return;
        if (!Validar()) return;

        // Actualizar propiedades — DonanteStore ya tiene la referencia por objeto
        _donante.NombreDonante = EntryNombre.Text!.Trim();
        _donante.TipoSangre = PickerSangre.SelectedItem!.ToString()!;
        _donante.Telefono = EntryTelefono.Text!.Trim();
        _donante.Direccion = EntryDireccion.Text!.Trim();
        _donante.IsAvailable = SwitchDisponible.IsToggled;
        _donante.UltimaDonacion = DateUltimaDonacion.Date;

        if (double.TryParse(EntryPeso.Text, out double peso))
            _donante.Peso = peso;

        if (LabelFechaNacimiento.Text != "DD/MM/AAAA")
        {
            DateTime fecha = DatePickerNacimiento.Date.GetValueOrDefault();
            _donante.FechaNacimiento = fecha;
            _donante.Edad = CalcularEdad(fecha);
        }

        if (_fotoPath != null)
            _donante.PhotoUrl = _fotoPath;

        await AppShell.DisplaySnackbarAsync($"{_donante.NombreDonante} actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    // ── Cancelar ──
    private async void OnCancelarClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    // ── Utilidad ──
    private static int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        int edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }
}