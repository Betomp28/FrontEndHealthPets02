using System;
using Microsoft.Maui.Controls;

namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class AgregarVacunasPage : ContentPage
    {
        public AgregarVacunasPage()
        {
            InitializeComponent();
        }

        private void OnAddVaccineClicked(object sender, EventArgs e)
        {
            string nombreVacuna = nombreVacunaEntry.Text;
            string descripcionVacuna = descripcionVacunaEntry.Text;
            DateTime fechaVencimiento = fechaVencimientoPicker.Date;

            // Validar los datos ingresados
            if (string.IsNullOrWhiteSpace(nombreVacuna))
            {
                DisplayAlert("Error", "Por favor ingrese el nombre de la vacuna.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(descripcionVacuna))
            {
                DisplayAlert("Error", "Por favor ingrese una descripción para la vacuna.", "OK");
                return;
            }

            if (fechaVencimiento == default)
            {
                DisplayAlert("Error", "Por favor seleccione una fecha de vencimiento.", "OK");
                return;
            }

            // Código para agregar la vacuna
            // Ejemplo: guardarla en una base de datos o en una lista

            DisplayAlert("Éxito", "Vacuna agregada exitosamente.", "OK");
            Navigation.PopAsync();
        }
    }
}
