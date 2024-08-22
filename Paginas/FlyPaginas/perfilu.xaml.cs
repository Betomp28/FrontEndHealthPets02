using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Modelos;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Net.Http.Headers;
using CommunityToolkit.Maui.Views;
using FrontEndHealthPets.Entidades;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using FrontEndHealthPets.Entidades.Request;
using FrontEndHealthPets.Entidades.response;


namespace FrontEndHealthPets.Paginas.FlyPaginas;
public partial class perfilu : ContentPage
{
    private readonly HttpClient _httpClient;
    public static string LaURL { get; } = "https://localhost:44348/api";
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

    private async void BtActualizarNombre_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aqu�
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Nombre");
            await this.ShowPopupAsync(popup);
        }
    }

    private async void BtActualizarApeliido_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aqu�
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Apellidos");
            await this.ShowPopupAsync(popup);
        }
    }

  
    private async void BtActualizarCooreo_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aqu�
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Correo");
            await this.ShowPopupAsync(popup);
        }
    }



    private async void BtActualizarPassword_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aqu�
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Password");
            await this.ShowPopupAsync(popup);
        }
    }

    public class EditarDatosPopup : Popup
    {
        private readonly UsuarioViewModel _viewModel;
        private readonly string _tipoEdicion;

        public EditarDatosPopup(UsuarioViewModel viewModel, string tipoEdicion)
        {
            _viewModel = viewModel;
            _tipoEdicion = tipoEdicion;

            var entry1 = new Entry { IsPassword = _tipoEdicion == "Password" };
            var entry2 = new Entry { IsPassword = _tipoEdicion == "Password" };

            var button = new Button { Text = "Aceptar", BackgroundColor = Colors.Purple, TextColor = Colors.White };

            string labelText, placeholder1, placeholder2 = null;
            switch (_tipoEdicion)
            {
                case "Nombre":
                    labelText = "Actualizar Nombre";
                    placeholder1 = "Nombre";
                    break;

                case "Apellidos":
                    labelText = "Actualizar Apellidos";
                    placeholder1 = "Apellidos";
                    break;

                case "Correo":
                    labelText = "Actualizar Correo Electr�nico";
                    placeholder1 = "Correo Electr�nico";
                    placeholder2 = "Confirmar Correo Electr�nico";
                    break;

                case "Password":
                    labelText = "Actualizar Contrase�a";
                    placeholder1 = "Nueva Contrase�a";
                    placeholder2 = "Confirmar Contrase�a";
                    break;

                default:
                    throw new ArgumentException("Tipo de edici�n no reconocido");
            }

            entry1.Placeholder = placeholder1;
            entry2.Placeholder = placeholder2;

            Content = new StackLayout
            {
                Padding = new Thickness(20),
                Children =
            {
                new Label { Text = labelText, FontSize = 20, FontAttributes = FontAttributes.Bold },
                entry1,
                entry2,
                button
            }
            };

            button.Clicked += async (sender, e) =>
            {
                var valor1 = entry1.Text;
                var valor2 = entry2.Text;

                if (string.IsNullOrEmpty(valor1) || (placeholder2 != null && string.IsNullOrEmpty(valor2)))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                    return;
                }

                if (placeholder2 != null && valor1 != valor2)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Los valores no coinciden.", "OK");
                    return;
                }
                // Preparar el objeto a enviar
                Req_Actualizar_Usuario req = new Req_Actualizar_Usuario();

                

                req.Id_Usuario = (int) Sesion.id_usuario;
                req.Nombre = _tipoEdicion == "Nombre" ? valor1 : null;
                req.Apellidos = _tipoEdicion == "Apellidos" ? valor1 : null;
                req.Correo_Electronico = _tipoEdicion == "Correo" ? valor1 : null;
                req.Confirmacion_Correo_Electronico = _tipoEdicion == "Correo" ? valor2 : null;
                req.Password = _tipoEdicion == "Password" ? valor1 : null;
                req.Confirmar_Password = _tipoEdicion == "Password" ? valor2 : null;

 
                  Debug.WriteLine($"Sesion.id_usuario: {Sesion.id_usuario}");



                try
                {
                    // Construir la URL con el par�metro de consulta
                    var requestUrl = $"{LaURL}/Usuarios/ActualizarUsuario/{Sesion.id_usuario}?Id_Usuario={Sesion.id_usuario}";
                    Debug.WriteLine($"Request URL: {requestUrl}");

                    // Verificar si la URL es v�lida
                    if (string.IsNullOrEmpty(LaURL))
                    {
                        Debug.WriteLine("Error: La URL est� vac�a o no es v�lida.");
                        await Application.Current.MainPage.DisplayAlert("Error", "La URL no es v�lida.", "OK");
                        return;
                    }

                    // Verificar si el ID de usuario es v�lido
                    if (Sesion.id_usuario == null)
                    {
                        Debug.WriteLine("Error: El ID de usuario es nulo.");
                        await Application.Current.MainPage.DisplayAlert("Error", "El ID de usuario no es v�lido.", "OK");
                        return;
                    }

                    // Realizar la solicitud PUT
                    using var httpClient = new HttpClient();
                    Debug.WriteLine("Enviando la solicitud PUT...");

                    var response = await httpClient.PutAsJsonAsync(requestUrl, req);
                    Debug.WriteLine($"Response Status Code: {response.StatusCode}");

                    // Verificar el c�digo de estado de la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Error: La solicitud fall� con el c�digo de estado {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Contenido de la respuesta de error: {errorContent}");
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar la informaci�n. C�digo de estado: {response.StatusCode}", "OK");
                        return;
                    }

                    response.EnsureSuccessStatusCode();

                    // Leer y depurar el contenido de la respuesta
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response JSON: {jsonString}");

                    // Intentar deserializar la respuesta JSON
                    try
                    {
                        var result = JsonConvert.DeserializeObject<Res_Actualizar_Usuario>(jsonString);
                        Debug.WriteLine($"Resultado de deserializaci�n: {JsonConvert.SerializeObject(result)}");

                        // Actualizar el ViewModel y la Sesion con los nuevos valores
                        switch (_tipoEdicion)
                        {
                            case "Nombre":
                                _viewModel.Nombre = valor1;
                                Sesion.nombre = _viewModel.Nombre;
                                break;

                            case "Apellidos":
                                _viewModel.Apellido = valor1;
                                Sesion.apellidos = _viewModel.Apellido;
                                break;

                            case "Correo":
                                _viewModel.CorreoElectronico = valor1;
                                Sesion.Correo_Electronico = _viewModel.CorreoElectronico;
                                break;

                            case "Password":
                                _viewModel.Password = valor1;
                                Sesion.Password = _viewModel.Password;
                                break;
                        }

                        // Mostrar confirmaci�n de �xito
                        await Application.Current.MainPage.DisplayAlert("�xito", "Datos actualizados correctamente.", "OK");
                        Close();
                    }
                    catch (JsonException jsonEx)
                    {
                        Debug.WriteLine($"Error de deserializaci�n: {jsonEx.Message}");
                        await Application.Current.MainPage.DisplayAlert("Error", "Hubo un problema al procesar la respuesta del servidor.", "OK");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Manejo de errores de la solicitud
                    Debug.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar los datos: {ex.Message}", "OK");
                }
                catch (Exception ex)
                {
                    // Manejo de cualquier otro tipo de excepci�n
                    Debug.WriteLine($"Excepci�n inesperada: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
                }
            };
        }
    }


    private async void bteliminar_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            // Depuraci�n: Comienza el m�todo
            System.Diagnostics.Debug.WriteLine("M�todo bteliminar_Clicked_1 iniciado");

            // Configuraci�n de la solicitud
            
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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sesion.token);

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
                System.Diagnostics.Debug.WriteLine($"URL utilizada: {LaURL}/Eliminar_Usuario/EliminarUsuario?Id_Usuario={correoElectronico}");

                // Enviar la solicitud DELETE al endpoint
                var response = await _httpClient.DeleteAsync($"{LaURL}/Eliminar_Usuario/EliminarUsuario?correoElectronico={correoElectronico}");

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

    private void Btcerrarsesion_Clicked(object sender, EventArgs e)
    {
        CerrarSesion();
 }

    private async void CerrarSesion()
    {
        
        Sesion.cerrarSesion();
        

        // Navegar de vuelta a la p�gina de inicio de sesi�n
        await Navigation.PushAsync(new MainPage());

        // Mostrar un mensaje de confirmaci�n
        await DisplayAlert("Cerrar sesi�n", "Has cerrado sesi�n correctamente.", "OK");
    }
}






