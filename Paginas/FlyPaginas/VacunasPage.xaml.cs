using System;
using Microsoft.Maui.Controls;

namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class VacunasPage : ContentPage
    {
        public VacunasPage()
        {
            InitializeComponent();
        }

        private async void OnAddVaccineClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgregarVacunasPage());
        }

        private void OnConsultVaccineClicked(object sender, EventArgs e)
        {
            // C�digo para consultar vacunas
            // Por ejemplo, podr�as mostrar una alerta o navegar a otra p�gina que muestre las vacunas existentes
            DisplayAlert("Consultar Vacunas", "Aqu� se mostrar�n las vacunas existentes.", "OK");
        }
    }
}
