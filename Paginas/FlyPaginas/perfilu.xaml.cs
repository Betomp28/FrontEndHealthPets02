using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Modelos;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
namespace FrontEndHealthPets.Paginas.FlyPaginas;
using System.Net.Http.Headers;

using FrontEndHealthPets.Entidades;
using Newtonsoft.Json;
using System.Net;
public partial class perfilu : ContentPage
{
    string LaURL = "https://localhost:44348/api";
    private UsuarioViewModel viewModel;
    public perfilu()
    {
        InitializeComponent();
        viewModel = new UsuarioViewModel
        {
            Nombre = Sesion.nombre,
            Apellido = Sesion.apellidos,
            CorreoElectronico = Sesion.Correo_Electronico, // Aseg�rate de tener este dato almacenado en la sesi�n si lo necesitas
            Password = Sesion.Password // Aseg�rate de tener este dato almacenado en la sesi�n si lo necesitas
        };


        // Debugging
        Debug.WriteLine($"Nombre: {Sesion.nombre}");
        Debug.WriteLine($"Apellido: {Sesion.apellidos}");
        Debug.WriteLine($"Correo: {Sesion.Correo_Electronico}");
        Debug.WriteLine($"Password: {Sesion.Password}");


        BindingContext = viewModel;
        Debug.WriteLine($"BindingContext: {BindingContext}");
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

    private void btRegresar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtActualizarNombre_Clicked(object sender, EventArgs e)
    {

    }

    private void BtActualizarApeliido_Clicked(object sender, EventArgs e)
    {

    }

    private void BtActualizarCooreo_Clicked(object sender, EventArgs e)
    {

    }

    private void BtActualizarPassword_Clicked(object sender, EventArgs e)
    {

    }




    private async void bteliminar_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            // Depuraci�n: Comienza el m�todo
            System.Diagnostics.Debug.WriteLine("M�todo bteliminar_Clicked_1 iniciado");

            // Configuraci�n de la solicitud
            using (HttpClient httpClient = new HttpClient())
            {
                // Supongamos que tienes un Entry en tu XAML con x:Name="CorreoEntry"
                string correoElectronico = CorreoEntry.Text;

                // Depuraci�n: Verifica el valor del correo electr�nico
                System.Diagnostics.Debug.WriteLine($"Correo electr�nico ingresado: {correoElectronico}");

                if (string.IsNullOrWhiteSpace(correoElectronico))
                {
                    await DisplayAlert("Error", "Por favor, ingresa un correo electr�nico.", "Aceptar");
                    System.Diagnostics.Debug.WriteLine("Correo electr�nico no proporcionado, se cancela la operaci�n.");
                    return;
                }

                // Agregar el token JWT en el encabezado Authorization
                System.Diagnostics.Debug.WriteLine($"Token JWT usado: {Sesion.token}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sesion.token);

                // Crear el objeto de solicitud
                Req_EliminarUsuario req = new Req_EliminarUsuario
                {
                    eliminar_usuario = new Req_EliminarUsuario.EliminarUsuarioDetails
                    {
                        correoElectronico = correoElectronico
                    }
                };

                // Depuraci�n: Verifica el objeto de solicitud
                System.Diagnostics.Debug.WriteLine($"Objeto de solicitud: {JsonConvert.SerializeObject(req)}");

                // Serializar el objeto a JSON
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");

                // Depuraci�n: Verifica el JSON que se va a enviar
                System.Diagnostics.Debug.WriteLine($"JSON enviado: {await jsonContent.ReadAsStringAsync()}");

                // Depuraci�n: URL usada para la solicitud DELETE
                System.Diagnostics.Debug.WriteLine($"URL utilizada: {LaURL}/api/Eliminar_Usuario/EliminarUsuario?Id_Usuario={correoElectronico}");

                // Enviar la solicitud DELETE al endpoint
                var response = await httpClient.DeleteAsync($"{LaURL}/api/Eliminar_Usuario/EliminarUsuario?Id_Usuario={correoElectronico}");

                // Depuraci�n: Verifica el c�digo de estado de la respuesta
                System.Diagnostics.Debug.WriteLine($"C�digo de estado de la respuesta: {response.StatusCode}");

                // Manejar la respuesta
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Contenido de la respuesta: {responseContent}");

                    Res_EliminarUsuario res = JsonConvert.DeserializeObject<Res_EliminarUsuario>(responseContent);

                    if (res.resultado)
                    {
                        System.Diagnostics.Debug.WriteLine("Usuario eliminado correctamente.");
                        await DisplayAlert("�xito", "Usuario eliminado correctamente", "Aceptar");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al eliminar usuario: {res.Error}");
                        await DisplayAlert("Error", res.Error ?? "No se pudo eliminar el usuario", "Aceptar");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("Error de autorizaci�n: Token inv�lido o expirado.");
                    await DisplayAlert("Autorizaci�n fallida", "El token no es v�lido o ha expirado", "Aceptar");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error de conexi�n: Ocurri� un error de conexi�n.");
                    await DisplayAlert("Error de conexi�n", "Ocurri� un error de conexi�n", "Aceptar");
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            // Depuraci�n: Error espec�fico de la solicitud HTTP
            System.Diagnostics.Debug.WriteLine($"Error HTTP: {httpEx.Message}");
            await DisplayAlert("Error HTTP", "Ocurri� un problema al enviar la solicitud: " + httpEx.Message, "Aceptar");
        }
        catch (Exception ex)
        {
            // Depuraci�n: Error gen�rico
            System.Diagnostics.Debug.WriteLine($"Error general: {ex.Message}");
            await DisplayAlert("Error de aplicaci�n", "Reinstale la aplicaci�n. Detalle: " + ex.Message, "Aceptar");
        }
        finally
        {
            // Depuraci�n: Finaliza el m�todo
            System.Diagnostics.Debug.WriteLine("M�todo bteliminar_Clicked_1 finalizado");
        }
    }
}






