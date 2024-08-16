using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.Response;
using FrontEndHealthPets.Entidades.Request;

namespace FrontEndHealthPets.Paginas;

public partial class Doctor : ContentPage
{

    public Doctor(string selectedDoctor)
	{
		InitializeComponent();
        lblSelectedDoctor.Text = $"Doctor seleccionado: {selectedDoctor}";
        if (selectedDoctor == "Dr. Juan P�rez")
        {
            lblTelefono.Text = "Tel�fono: 555-1234";
            lblCorreo_Doctor.Text = "Correo electr�nico: jperez@hospital.com";
        }
    }

}