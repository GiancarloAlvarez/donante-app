using Donant_app.Models;

namespace Donant_app.Pages;

public partial class NewDonorPage : ContentPage
{
    private string? _fotoPath = null;

    public NewDonorPage()
    {
        InitializeComponent();
    }

    // ── Foto ────────────────────────────────────────────────
    private async void OnFotoTapped(object sender, EventArgs e)
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result != null)
        {
            _fotoPath = result.FullPath;
            ImgFoto.Source = ImageSource.FromFile(_fotoPath);
            ImgFoto.IsVisible = true;
            LblIconoFoto.IsVisible = false;
        }
    }

    // ── Fecha de nacimiento ─────────────────────────────────
    private void OnFechaNacimientoSelected(object sender, DateChangedEventArgs e)
    {
        DateTime fechaNac = e.NewDate.GetValueOrDefault();
        LabelFechaNacimiento.Text = fechaNac.ToString("dd/MM/yyyy");
        LabelFechaNacimiento.TextColor = Color.FromArgb("#1F2937");
    }

    private int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        int edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento.Date > hoy.AddYears(-edad))
            edad--;
        return edad;
    }

    // ── Guardar ─────────────────────────────────────────────
    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(EntryNombre.Text))
        {
            await AppShell.DisplaySnackbarAsync("El nombre es obligatorio.");
            return;
        }

        if (PickerSangre.SelectedIndex == -1)
        {
            await AppShell.DisplaySnackbarAsync("Selecciona el tipo de sangre.");
            return;
        }

        if (LabelFechaNacimiento.Text == "DD/MM/AAAA")
        {
            await AppShell.DisplaySnackbarAsync("Selecciona la fecha de nacimiento.");
            return;
        }

        DateTime fechaNac = DatePickerNacimiento.Date.GetValueOrDefault();
        int edadCalculada = CalcularEdad(fechaNac);

        var nuevoDonante = new Donante
        {
            Id = DonanteStore.Donantes.Count + 1,
            NombreDonante = EntryNombre.Text.Trim(),
            TipoSangre = PickerSangre.SelectedItem?.ToString() ?? string.Empty,
            FechaNacimiento = fechaNac,
            Edad = edadCalculada,
            Telefono = EntryTelefono.Text?.Trim() ?? string.Empty,
            Direccion = EntryDireccion.Text?.Trim() ?? string.Empty,
            UltimaDonacion = DateUltimaDonacion.Date.GetValueOrDefault(),
            Peso = double.TryParse(EntryPeso.Text, out double peso) ? peso : 0,
            IsAvailable = SwitchDisponible.IsToggled,
            PhotoUrl = _fotoPath,
            CreatedAt = DateTime.Now
        };

        DonanteStore.Donantes.Add(nuevoDonante);

        await AppShell.DisplaySnackbarAsync(
            $"✓ {nuevoDonante.NombreDonante} registrado · {edadCalculada} años");

        await Shell.Current.GoToAsync("..");
    }

    // ── Cancelar ────────────────────────────────────────────
    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}