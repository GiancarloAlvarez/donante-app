using Donant_app.Models;

namespace Donant_app.Pages;

[QueryProperty(nameof(Section), "section")]
public partial class ProjectListPage : ContentPage
{
    private string _section = "all";

    public string Section
    {
        get => _section;
        set => _section = string.IsNullOrWhiteSpace(value) ? "all" : value.ToLowerInvariant();
    }

    public ProjectListPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DataReady;
        CargarDonantes(DonanteStore.Donantes);
        BindableLayout.SetItemsSource(
            ListaSolicitudes,
            SolicitudSangreStore.Solicitudes
                .OrderBy(request => request.EstaAtendida)
                .ThenByDescending(request => request.FechaSolicitud)
                .ToList());
        BindableLayout.SetItemsSource(ListaHistorial, RegistroDonacionStore.Registros);
        ContadorSolicitudes.Text = $"({SolicitudSangreStore.Solicitudes.Count})";
        ContadorHistorial.Text = $"({RegistroDonacionStore.Registros.Count})";
        await ScrollToSectionAsync(Section, false);
    }

    private void CargarDonantes(IReadOnlyList<Donante> lista)
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

    private void OnBuscar(object? sender, TextChangedEventArgs e)
    {
        CargarDonantes(DonanteStore.Search(e.NewTextValue));
    }

    private async void OnFabTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewDonorPage));
    }

    private async void OnAllTabTapped(object? sender, TappedEventArgs e) =>
        await ScrollToSectionAsync("all");

    private async void OnDonorsTabTapped(object? sender, TappedEventArgs e) =>
        await ScrollToSectionAsync("donors");

    private async void OnRequestsTabTapped(object? sender, TappedEventArgs e) =>
        await ScrollToSectionAsync("requests");

    private async void OnHistoryTabTapped(object? sender, TappedEventArgs e) =>
        await ScrollToSectionAsync("history");

    private async void OnFilterTapped(object? sender, TappedEventArgs e)
    {
        string? choice = await DisplayActionSheetAsync(
            "Ir a una sección",
            "Cancelar",
            null,
            "Todos",
            "Donantes",
            "Citas y solicitudes",
            "Historial");

        string? section = choice switch
        {
            "Todos" => "all",
            "Donantes" => "donors",
            "Citas y solicitudes" => "requests",
            "Historial" => "history",
            _ => null
        };

        if (section is not null)
        {
            await ScrollToSectionAsync(section);
        }
    }

    private async Task ScrollToSectionAsync(string section, bool animate = true)
    {
        Section = section;
        VisualElement target = Section switch
        {
            "donors" => DonorsSection,
            "requests" => RequestsSection,
            "history" => HistorySection,
            _ => ContentRoot
        };

        SetActiveTab(Section);
        await Task.Yield();
        await MainScrollView.ScrollToAsync(target, ScrollToPosition.Start, animate);
    }

    private void SetActiveTab(string section)
    {
        (Border Border, Label Label, string Key)[] tabs =
        [
            (TabAll, TabAllLabel, "all"),
            (TabDonors, TabDonorsLabel, "donors"),
            (TabRequests, TabRequestsLabel, "requests"),
            (TabHistory, TabHistoryLabel, "history")
        ];

        foreach ((Border border, Label label, string key) in tabs)
        {
            bool selected = key == section;
            border.BackgroundColor = selected ? Color.FromArgb("#DC143C") : Colors.White;
            label.TextColor = selected ? Colors.White : Color.FromArgb("#4B5563");
            label.FontFamily = selected ? "InterSemiBold" : "InterMedium";
        }
    }

    private async void OnSolicitudTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not SolicitudSangre solicitud)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(SolicitudDetallePage),
            new Dictionary<string, object> { ["Solicitud"] = solicitud });
    }

    // Ver perfil
    private async void OnDonanteTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Donante donante)
        {
            await Shell.Current.GoToAsync(nameof(DonantePage),
                new Dictionary<string, object> { ["Donante"] = donante });
        }
    }

    // Editar
    private async void OnEditarDonanteTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Donante donante)
        {
            await Shell.Current.GoToAsync(nameof(EditDonorPage),
                new Dictionary<string, object> { ["Donante"] = donante });
        }
    }

    // Eliminar
    private async void OnEliminarDonanteTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not Donante donante) return;

        bool confirmar = await DisplayAlertAsync(
            "Eliminar donante",
            $"¿Eliminar a {donante.NombreDonante}?",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        try
        {
            await DonanteStore.DeleteAsync(donante.Id);
            RefrescarLista();
            await AppShell.DisplaySnackbarAsync($"{donante.NombreDonante} eliminado.");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("No se pudo eliminar", ex.Message, "Entendido");
        }
    }

    private void RefrescarLista()
    {
        CargarDonantes(DonanteStore.Search(EntryBuscar.Text));
    }
}
