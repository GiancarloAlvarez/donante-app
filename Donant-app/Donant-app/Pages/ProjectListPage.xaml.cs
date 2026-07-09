using Donant_app.Models;

namespace Donant_app.Pages;

public partial class ProjectListPage : ContentPage
{
    public ProjectListPage()
    {
        InitializeComponent();
        CargarDonantes(DonanteStore.Donantes);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarDonantes(DonanteStore.Donantes);
    }

    private void CargarDonantes(List<Donante> lista)
    {
        DonorsCollectionView.ItemsSource = null; 
        DonorsCollectionView.ItemsSource = lista;
        var contador = this.FindByName<Microsoft.Maui.Controls.Label>("ContadorDonantes");
        if (contador != null)
        {
            contador.Text = $"({lista.Count})";
        }
        var emptyLabel = this.FindByName<Microsoft.Maui.Controls.Label>("EmptyLabel");
        if (emptyLabel != null)
        {
            emptyLabel.IsVisible = lista.Count == 0;
        }
    }

    private void OnBuscar(object sender, TextChangedEventArgs e)
    {
        var texto = e.NewTextValue?.ToLower() ?? string.Empty;

        var filtrados = DonanteStore.Donantes
            .Where(d => d.NombreDonante.ToLower().Contains(texto)
                     || d.TipoSangre.ToLower().Contains(texto)
                     || d.Direccion.ToLower().Contains(texto))
            .ToList();

        CargarDonantes(filtrados);
    }

    private async void OnFabTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewDonorPage));
    }
}