using Donant_app.Models;
using Donant_app.PageModels;

namespace Donant_app.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}