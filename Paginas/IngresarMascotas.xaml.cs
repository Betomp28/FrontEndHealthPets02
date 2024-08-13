namespace FrontEndHealthPets.Paginas;

using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.Request;
using FrontEndHealthPets.Entidades.response;
using FrontEndHealthPets.Paginas.FlyPaginas;
using Newtonsoft.Json;
using System.Diagnostics;

public partial class IngresarMascotas : ContentPage
{
    string LaURL = "https://localhost:44348/api";
    public IngresarMascotas()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            // Abre la galer�a para seleccionar una imagen
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una foto de perfil",
                FileTypes = FilePickerFileType.Images // Filtra solo im�genes
            });

            if (result != null)
            {
                // Muestra la imagen seleccionada en el control Image
                using var stream = await result.OpenReadAsync();
                imgPerfil.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar la imagen: " + ex.Message, "OK");
        }
    }

    private async void btRegistrar_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validar Fecha de Nacimiento
            DateTime fechaNacimiento = Fecha_Nacimiento.Date;

            if (string.IsNullOrEmpty(Nombre.Text))
            {
                Debug.WriteLine("Error: Nombre vac�o");
                DisplayAlert("Error", "Nombre vac�o", "Aceptar");
                return;
            }
            else if (string.IsNullOrEmpty(Especie.Text))
            {
                Debug.WriteLine("Error: Especie vac�a");
                DisplayAlert("Error", "Especie vac�a", "Aceptar");
                return;
            }
            else
            if (string.IsNullOrEmpty(Raza.Text))
            {
                Debug.WriteLine("Error: Raza vac�a");
                DisplayAlert("Error", "Raza vac�a", "Aceptar");
                return;
            }
            
            else if (fechaNacimiento == DateTime.MinValue)
            {
                await DisplayAlert("Error", "Por favor, selecciona una fecha de nacimiento v�lida.", "OK");
                return;
            }


            Req_Mascota req = new Req_Mascota();

            req.Registro_Mascota = new Registro_Mascota();

            req.Registro_Mascota.id_Usuario = Sesion.id_usuario;
            Debug.WriteLine($"Nombre recibido: {req.Registro_Mascota.Nombre}, ID Usuario recibido: {req.Registro_Mascota.id_Usuario}");

            req.Registro_Mascota.Nombre = Nombre.Text;
            Debug.WriteLine($"Nombre recibido: {req.Registro_Mascota.Nombre}");

            req.Registro_Mascota.especie = Especie.Text;
            Debug.WriteLine($"Especie recibida: {req.Registro_Mascota.especie}");

            req.Registro_Mascota.raza = Raza.Text;
            Debug.WriteLine($"Raza recibida: {req.Registro_Mascota.raza}");

            req.Registro_Mascota.Fecha_Nacimiento = fechaNacimiento;
            Debug.WriteLine($"Fecha de Nacimiento recibida: {req.Registro_Mascota.Fecha_Nacimiento}");

            var json = JsonConvert.SerializeObject(req);
            Debug.WriteLine("JSON enviado: " + json);

            var jsoncontent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync(LaURL + "/Registro_Mascota/IngresarMascota", jsoncontent);
            Debug.WriteLine("C�digo de estado: " + response.StatusCode);
            if (response.IsSuccessStatusCode)// saber si el api esta vivo
            {
                // si conecto
                string responseContent = await response.Content.ReadAsStringAsync();
                var responsecontent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Contenido de la respuesta: " + responseContent);

                Res_Mascota res = new Res_Mascota();

                res = JsonConvert.DeserializeObject<Res_Mascota>(responsecontent);

                if (res.resultado)
                {
                    DisplayActionSheet("Registro", "Usuario Registrado", "Aceptar");
                    Navigation.PushAsync(new MisMascotas());

                }
                else
                {
                    DisplayAlert("Error", res.Error, "Aceptar");// Error en backend", "Login incorrecto!", "Aceptar" agregar luego

                }
            }
            else
            {
                //No conect�
                DisplayAlert("Error de conexi�n", "Ocurri� un error de conexi�n", "Aceptar");

            }
        }

        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Aceptar");
        }

    }

        private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MisMascotas());
    }
}