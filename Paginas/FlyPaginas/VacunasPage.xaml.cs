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
            // Código para consultar vacunas
            // Por ejemplo, podrías mostrar una alerta o navegar a otra página que muestre las vacunas existentes
            DisplayAlert("Consultar Vacunas", "Aquí se mostrarán las vacunas existentes.", "OK");
        }
    }
}
