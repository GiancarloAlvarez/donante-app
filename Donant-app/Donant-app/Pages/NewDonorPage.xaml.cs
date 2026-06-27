// NewDonorPage.xaml.cs
using Donant_app.Models;

namespace Donant_app.Pages;

public partial class NewDonorPage : ContentPage
{
    private string? _fotoPath = null;

    public NewDonorPage()
    {
        InitializeComponent();
    }

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

    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNombre.Text))
        {
            await DisplayAlert("Error", "El nombre es obligatorio", "OK");
            return;
        }

        var nuevoDonante = new Donante
        {
            Id = DonanteStore.Donantes.Count + 1,
            NombreDonante = EntryNombre.Text.Trim(),
            TipoSangre = PickerSangre.SelectedItem?.ToString() ?? string.Empty,
            Edad = int.TryParse(EntryEdad.Text, out int edad) ? edad : 0,
            Telefono = EntryTelefono.Text?.Trim() ?? string.Empty,
            Direccion = EntryDireccion.Text?.Trim() ?? string.Empty,
            UltimaDonacion = DateUltimaDonacion.Date,
            Peso = double.TryParse(EntryPeso.Text, out double peso) ? peso : 0,
            IsAvailable = SwitchDisponible.IsToggled,
            PhotoUrl = _fotoPath,
            CreatedAt = DateTime.Now
        };

        // ✅ Guardar en el store compartido
        DonanteStore.Donantes.Add(nuevoDonante);

        await DisplayAlert("Éxito", $"'{nuevoDonante.NombreDonante}' registrado", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}