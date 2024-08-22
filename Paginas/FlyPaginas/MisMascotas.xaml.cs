using FrontEndHealthPets.Modelos;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class MisMascotas : ContentPage
{
	public MisMascotas()
	{
		InitializeComponent();
        BindingContext = new MascotasViewModel(); // Establecer el BindingContext
    }

    private void btNuevaMascota_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new IngresarMascotas());
    }
}