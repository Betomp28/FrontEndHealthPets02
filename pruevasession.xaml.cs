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
            CorreoElectronico = Sesion.Correo_Electronico // Aseg�rate de tener este dato almacenado en la sesi�n si lo necesitas
        };
        BindingContext = viewModel;
    }
}

private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}
