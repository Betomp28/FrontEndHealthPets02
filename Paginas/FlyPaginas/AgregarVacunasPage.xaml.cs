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
            string nombre = nombreVacunaEntry.Text;
            string descripcion = descripcionVacunaEditor.Text;  // Aseg�rate de usar el nombre correcto del Editor
            DateTime fechaVencimiento = fechaVencimientoPicker.Date;

            // Implementar l�gica para guardar los datos de la vacuna
        }
    }
}
