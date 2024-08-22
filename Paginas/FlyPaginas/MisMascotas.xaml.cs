using FrontEndHealthPets.Modelos;
using Microsoft.Maui.Controls;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class MisMascotas : ContentPage
{
    public MisMascotas()
    {
        InitializeComponent();
        BindingContext = new MascotasViewModel(); // Establecer el BindingContext
    }

    private async void MascotaSeleccionada(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is PerfilMascota mascotaSeleccionada)
        {
            await Navigation.PushAsync(new DetallesMascotaPage(mascotaSeleccionada));
        }
    }

    private void btNuevaMascota_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new IngresarMascotas());
    }
}
