using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Modelos;

namespace FrontEndHealthPets;

public partial class pruevasession : ContentPage
{
    private UsuarioViewModel viewModel;

    public pruevasession()
    {
        InitializeComponent();
        viewModel = new UsuarioViewModel
        {
            Nombre = Sesion.nombre,
            Apellido = Sesion.apellidos,
            CorreoElectronico = Sesion.Correo_Electronico, // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
            Password = Sesion.Password // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
        };
        BindingContext = viewModel;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        // Lógica para manejar el evento tap
    }
}