using Donant_app.Models;
using Donant_app.ViewModels;

namespace Donant_app.Pages;

public partial class NewDonorPage : ContentPage
{
    private string? _fotoPath;

    public NewDonorPage()
    {
        InitializeComponent();
    }

    private async void OnFotoTapped(object? sender, EventArgs e)
    {
        _fotoPath = await PhotoStorage.PickAndSavePhotoAsync("Selecciona una foto del donante");
        if (_fotoPath is not null)
        {
            ImgFoto.Source = ImageSource.FromFile(_fotoPath);
            ImgFoto.IsVisible = true;
            LblIconoFoto.IsVisible = false;

            if (BindingContext is NewDonorViewModel vm)
            {
                vm.PhotoPath = _fotoPath;
            }
        }
    }

    private void OnFechaNacimientoSelected(object? sender, DateChangedEventArgs e)
    {
        DateTime fechaNac = e.NewDate.GetValueOrDefault();
        LabelFechaNacimiento.Text = fechaNac.ToString("dd/MM/yyyy");
        LabelFechaNacimiento.TextColor = Color.FromArgb("#1F2937");
    }

    private async void OnGuardarTapped(object? sender, EventArgs e)
    {
        if (BindingContext is NewDonorViewModel vm)
            await vm.GuardarCommand.ExecuteAsync(null);
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        if (BindingContext is NewDonorViewModel vm)
            vm.CancelarCommand.Execute(null);
        await Shell.Current.GoToAsync("//main");
    }
}
