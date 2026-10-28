using Donant_app.Models;

namespace Donant_app.Pages;

public partial class CentrosDonacionPage : ContentPage
{
    public CentrosDonacionPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DataReady;

        List<CentroDonacionItem> centers = RegistroDonacionStore.Registros
            .Where(record => !string.IsNullOrWhiteSpace(record.CentroDonacion))
            .GroupBy(
                record => $"{record.CentroDonacion.Trim()}|{record.Ciudad.Trim()}",
                StringComparer.OrdinalIgnoreCase)
            .Select(group => new CentroDonacionItem
            {
                Nombre = group.First().CentroDonacion.Trim(),
                Ciudad = group.First().Ciudad.Trim(),
                Donaciones = group.Count(),
                UltimaDonacion = group.Max(record => record.FechaDonacion)
            })
            .OrderBy(center => center.Nombre)
            .ToList();

        CentersCollectionView.ItemsSource = centers;
        LblCenterCount.Text = centers.Count == 1
            ? "1 centro en tu historial"
            : $"{centers.Count} centros en tu historial";
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnRegisterDonationTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//registro");
}
