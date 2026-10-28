using Donant_app.Models;

namespace Donant_app.Pages;

[QueryProperty(nameof(Donante), "Donante")]
public partial class HistorialDonantePage : ContentPage
{
    private Donante? _donante;

    public Donante? Donante
    {
        get => _donante;
        set => _donante = value;
    }

    public HistorialDonantePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DataReady;

        if (_donante is null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        LblDonorName.Text = _donante.NombreDonante;
        HistoryCollectionView.ItemsSource = RegistroDonacionStore.Registros
            .Where(record => record.DonanteId == _donante.Id)
            .OrderByDescending(record => record.FechaDonacion)
            .ToList();
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");
}
