using FrontEndHealthPets.Entidades.Request;
using Newtonsoft.Json;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.Response;
using Newtonsoft.Json;
using System.Net.Http;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class ClinicaVeterinaria : ContentPage
{
    string laURL = "https://localhost:44348/api";

    public ClinicaVeterinaria()
    {
        InitializeComponent();

}

private void PickerClinicas_SelectedIndexChanged(object sender, EventArgs e)
    {
        var clinica = pickerClinicas.Items[pickerClinicas.SelectedIndex];
        if (clinica == "Clínica Veterinaria San Francisco")
        {
            lblDireccion.Text = "Direccion: Calle 1";
            lblTelefono.Text = "Telefono: 123456";
        }
        else if (clinica == "Clínica Veterinaria Los Ángeles")
        {
            lblDireccion.Text = "Direccion: Calle 2";
            lblTelefono.Text = "Telefono: 654321";
        }
        else if (clinica == "Clínica Veterinaria El Bosque")
        {
            lblDireccion.Text = "Direccion: Calle 3";
            lblTelefono.Text = "Telefono: 987654";
        }
        else if (clinica == "Clínica Veterinaria La Pradera")
        {
            lblDireccion.Text = "Direccion: Calle 4";
            lblTelefono.Text = "Telefono: 987654";
        }
        else if (clinica == "Clínica Veterinaria El Roble")
        {
            lblDireccion.Text = "Direccion: Calle 5";
            lblTelefono.Text = "Telefono: 987654";
        }

    }
   

    private void PickerDoctor_SelectedIndexChanged(object sender, EventArgs e)
    {


    }



    private async void Agregar_Clicked(object sender, EventArgs e)
    {
        string selectedDoctor = pickerDoctor.SelectedItem as string;
        await Navigation.PushAsync(new Doctor(selectedDoctor));

        //    try
        //    {
        //        if (pickerClinicas.SelectedIndex == -1)
        //        {
        //            await DisplayAlert("Error", "Debe seleccionar una clínica", "Aceptar");
        //            return;
        //        }
        //        if (pickerDoctor.SelectedIndex == -1)
        //        {
        //            await DisplayAlert("Error", "Debe seleccionar un doctor", "Aceptar");
        //            return;
        //        }

        //        Req_Clinica_Veterinaria req = new Req_Clinica_Veterinaria();

        //        req.clinica_Veterinaria = new Clinica_Veterinaria();

        //        req.clinica_Veterinaria.Nombre_Clinica = pickerClinicas.SelectedItem.ToString();
        //        req.clinica_Veterinaria.Nombre_Doctor = pickerDoctor.SelectedItem.ToString();
        //        req.clinica_Veterinaria.Direccion = lblDireccion.Text;
        //        req.clinica_Veterinaria.Telefono = lblTelefono.Text;



        //        var jsoncontent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");
        //         HttpClient httpClient = new HttpClient();

        //var response = await httpClient.PostAsync(laURL + "/Ingresar_Datos_CLinica/Ingresar_Citas_Datos_CLinica", jsoncontent);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responsecontent = await response.Content.ReadAsStringAsync();
        //            var res = JsonConvert.DeserializeObject<Res_Clinica_Veterinaria>(responsecontent);

        //            if (res.resultado)
        //            {
        //                await DisplayActionSheet("Registro", "Usuario Registrado", "Aceptar");
        //                await Navigation.PushAsync(new MainPage());
        //            }
        //            else
        //            {
        //                await DisplayAlert("Error", res.Error, "Aceptar");
        //            }
        //        }
        //        else
        //        {
        //            var errorMessage = await response.Content.ReadAsStringAsync();
        //            await DisplayAlert("Error de conexión", $"Código de estado: {response.StatusCode}\n{errorMessage}", "Aceptar");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", ex.Message, "Aceptar");
        //    }
    }

}

