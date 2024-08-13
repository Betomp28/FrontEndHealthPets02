using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Modelos;
using System.Diagnostics;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class perfilu : ContentPage
{
    private UsuarioViewModel viewModel;
    public perfilu()
	{
		InitializeComponent();
        viewModel = new UsuarioViewModel
        {
            Nombre = Sesion.nombre,
            Apellido = Sesion.apellidos,
            CorreoElectronico = Sesion.Correo_Electronico, // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
            Password = Sesion.Password // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
        };


        // Debugging
        Debug.WriteLine($"Nombre: {Sesion.nombre}");
        Debug.WriteLine($"Apellido: {Sesion.apellidos}");
        Debug.WriteLine($"Correo: {Sesion.Correo_Electronico}");
        Debug.WriteLine($"Password: {Sesion.Password}");


        BindingContext = viewModel;
        Debug.WriteLine($"BindingContext: {BindingContext}");
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void btRegresar_Clicked(object sender, EventArgs e)
    {

    }
}
