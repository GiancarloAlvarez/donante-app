using Donant_app.ViewModels;

namespace Donant_app.Pages;

public partial class RegistroDonacionPage : ContentPage
{
    public RegistroDonacionPage()
    {
        InitializeComponent();
    }

    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        if (BindingContext is RegistroDonacionViewModel vm)
            await vm.GuardarCommand.ExecuteAsync(null);
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}