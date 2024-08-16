using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.Request;
using FrontEndHealthPets.Entidades.Response;
using FrontEndHealthPets.Paginas;
using FrontEndHealthPets.Paginas.FlyPaginas;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FrontEndHealthPets
{
    public partial class MainPage : ContentPage
    {
        string laURL = "https://localhost:44348/api";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void btiniciarsecion_Clicked(object sender, EventArgs e)
        {



           
        }





        private void btregistrarse_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Registro());
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {

        }
    }
}

