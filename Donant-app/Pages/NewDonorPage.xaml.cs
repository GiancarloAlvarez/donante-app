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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is NewDonorViewModel { PhotoPath: null })
        {
            ResetPhotoPreview();
        }
    }

    private async void OnFotoTapped(object? sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            await DisplayAlertAsync("No se pudo cargar la foto", ex.Message, "Entendido");
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
        ResetPhotoPreview();
        await Shell.Current.GoToAsync("//main");
    }

    private void ResetPhotoPreview()
    {
        _fotoPath = null;
        ImgFoto.Source = null;
        ImgFoto.IsVisible = false;
        LblIconoFoto.IsVisible = true;
    }
}
