using Donant_app.ViewModels;

namespace Donant_app.Pages;

public partial class RegistroDonacionPage : ContentPage
{
    public RegistroDonacionPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RegistroDonacionViewModel vm)
        {
            await vm.CargarDonantesAsync();
        }
    }

    private async void OnGuardarTapped(object? sender, EventArgs e)
    {
        if (BindingContext is RegistroDonacionViewModel vm)
            await vm.GuardarCommand.ExecuteAsync(null);
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        if (BindingContext is RegistroDonacionViewModel vm)
            vm.CancelarCommand.Execute(null);
        await Shell.Current.GoToAsync("//main");
    }
}
