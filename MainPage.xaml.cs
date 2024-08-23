using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.Request;
using FrontEndHealthPets.Entidades.Response;
using FrontEndHealthPets.Paginas;
using FrontEndHealthPets.Paginas.FlyPaginas;
using FrontEndHealthPets.Paginas.tabpage;

using Newtonsoft.Json;
using System.Diagnostics;

namespace FrontEndHealthPets
{
    public partial class MainPage : ContentPage
    {
        string laURL = "https://localhost:44348/api";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void btiniciarsecion_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Validación de campos de inicio de sesión
                if (string.IsNullOrWhiteSpace(Correo.Text))
                {
                    await DisplayAlert("Error", "Por favor, ingresa tu usuario o correo electrónico.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Passwoord.Text))
                {
                    await DisplayAlert("Error", "Por favor, ingresa tu contraseña.", "OK");
                    return;
                }

                Req_Login req = new Req_Login
                {
                    Correo_Electronico = Correo.Text,
                    Contrasena = Passwoord.Text
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    var responseTask = httpClient.PostAsync($"{laURL}/login/IngresarLogin", jsonContent);
                    var response = await responseTask;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Res_Login res = JsonConvert.DeserializeObject<Res_Login>(responseContent);


                        if (res.resultado)
                        {

                            Sesion.id_usuario = res.Registro_Usuario.Id_Usuario;
                            Sesion.nombre = res.Registro_Usuario.Nombre;
                            Sesion.apellidos = res.Registro_Usuario.Apellidos;
                            Sesion.Correo_Electronico = res.Registro_Usuario.Correo_Electronico;
                            Sesion.Password = res.Registro_Usuario.Password;
                            Sesion.token = res.Registro_Usuario.token;

                            Debug.WriteLine($"Correo_Electronico: {Sesion.Correo_Electronico}");
                            Debug.WriteLine($"Password: {Sesion.Password}");

                            await Navigation.PushAsync(new PagFlyPrincipal());
                        }
                        else
                        {
                            await DisplayAlert("Error en backend", "Login incorrecto!", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert($"Error de conexión", "Ocurrió un error de conexión", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de aplicación", "Reinstale la aplicación. Detalle: " + ex.Message, "Aceptar");
            }

        }








        private void btregistrarse_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Registro());
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {

        }
    }
}