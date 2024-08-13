using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;

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
            CorreoElectronico = Sesion.Correo_Electronico // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
        };
        BindingContext = viewModel;
    }
}

private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}
