namespace Donant_app.Pages;

public partial class NewDonorPage : ContentPage
{
	public NewDonorPage()
	{
		InitializeComponent();
	}

    private async void OnGuardarTapped(object sender, EventArgs e)
    {
        // lógica de guardado
        await Shell.Current.GoToAsync("..");
    }
}