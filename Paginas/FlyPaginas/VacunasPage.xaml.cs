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

        private void OnAddVaccineClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregarVacunasPage());
        }

        private void OnAssignVaccineToPetClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AsignarVacunaMascotaPage());
        }

        private void OnConsultVaccinesClicked(object sender, EventArgs e)
        {
            // Implementar la lógica para consultar las vacunas
        }
    }
}
