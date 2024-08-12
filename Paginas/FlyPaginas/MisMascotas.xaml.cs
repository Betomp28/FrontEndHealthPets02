namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class MisMascotas : ContentPage
{
	public MisMascotas()
	{
		InitializeComponent();
	}

    private void btNuevaMascota_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new IngresarMascotas());
    }
}