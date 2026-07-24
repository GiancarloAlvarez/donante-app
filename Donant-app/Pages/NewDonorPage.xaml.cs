using Donant_app.Models;
using Donant_app.ViewModels;

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

    private void OnFechaNacimientoSelected(object sender, DateChangedEventArgs e)
    {
        DateTime fechaNac = e.NewDate.GetValueOrDefault();
        LabelFechaNacimiento.Text = fechaNac.ToString("dd/MM/yyyy");
        LabelFechaNacimiento.TextColor = Color.FromArgb("#1F2937");
    }

    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        if (BindingContext is NewDonorViewModel vm)
            await vm.GuardarCommand.ExecuteAsync(null);
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        if (BindingContext is NewDonorViewModel vm)
            vm.CancelarCommand.Execute(null);
        await Shell.Current.GoToAsync("//main");
    }
}