using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

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
    private async void Button_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new MainPage());
    }

}