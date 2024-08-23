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
using FrontEndHealthPets.Entidades.entitys;


namespace FrontEndHealthPets.Paginas.FlyPaginas;
public partial class perfilu : ContentPage
{
    private readonly HttpClient _httpClient;
    public static string LaURL { get; } = "https://localhost:44348/api";
    private UsuarioViewModel viewModel;
    public perfilu()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        viewModel = new UsuarioViewModel
        {
            Nombre = Sesion.nombre,
            Apellido = Sesion.apellidos,
            CorreoElectronico = Sesion.Correo_Electronico, // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
            Password = Sesion.Password // Asegúrate de tener este dato almacenado en la sesión si lo necesitas
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
            // Abre la galería para seleccionar una imagen
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una foto de perfil",
                FileTypes = FilePickerFileType.Images // Filtra solo imágenes
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
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aquí
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Nombre");
            await this.ShowPopupAsync(popup);
        }
    }

    private async void BtActualizarApeliido_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aquí
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Apellidos");
            await this.ShowPopupAsync(popup);
        }
    }


    private async void BtActualizarCooreo_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aquí
        if (viewModel != null)
        {
            var popup = new EditarDatosPopup(viewModel, "Correo");
            await this.ShowPopupAsync(popup);
        }
    }



    private async void BtActualizarPassword_Clicked(object sender, EventArgs e)
    {
        var viewModel = (UsuarioViewModel)this.BindingContext; // Usa UsuarioViewModel aquí
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
                    labelText = "Actualizar Correo Electrónico";
                    placeholder1 = "Correo Electrónico";
                    placeholder2 = "Confirmar Correo Electrónico";
                    break;

                case "Password":
                    labelText = "Actualizar Contraseña";
                    placeholder1 = "Nueva Contraseña";
                    placeholder2 = "Confirmar Contraseña";
                    break;

                default:
                    throw new ArgumentException("Tipo de edición no reconocido");
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



                req.Id_Usuario = (int)Sesion.id_usuario;
                req.Nombre = _tipoEdicion == "Nombre" ? valor1 : null;
                req.Apellidos = _tipoEdicion == "Apellidos" ? valor1 : null;
                req.Correo_Electronico = _tipoEdicion == "Correo" ? valor1 : null;
                req.Confirmacion_Correo_Electronico = _tipoEdicion == "Correo" ? valor2 : null;
                req.Password = _tipoEdicion == "Password" ? valor1 : null;
                req.Confirmar_Password = _tipoEdicion == "Password" ? valor2 : null;


                Debug.WriteLine($"Sesion.id_usuario: {Sesion.id_usuario}");



                try
                {
                    // Construir la URL con el parámetro de consulta
                    var requestUrl = $"{LaURL}/Usuarios/ActualizarUsuario/{Sesion.id_usuario}?Id_Usuario={Sesion.id_usuario}";
                    Debug.WriteLine($"Request URL: {requestUrl}");

                    // Verificar si la URL es válida
                    if (string.IsNullOrEmpty(LaURL))
                    {
                        Debug.WriteLine("Error: La URL está vacía o no es válida.");
                        await Application.Current.MainPage.DisplayAlert("Error", "La URL no es válida.", "OK");
                        return;
                    }

                    // Verificar si el ID de usuario es válido
                    if (Sesion.id_usuario == null)
                    {
                        Debug.WriteLine("Error: El ID de usuario es nulo.");
                        await Application.Current.MainPage.DisplayAlert("Error", "El ID de usuario no es válido.", "OK");
                        return;
                    }

                    // Realizar la solicitud PUT
                    using var httpClient = new HttpClient();
                    Debug.WriteLine("Enviando la solicitud PUT...");

                    // Añadir el token Bearer a los encabezados
                    string token = Sesion.token; // Asumiendo que el token está guardado en Sesion
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Debug.WriteLine($"Bearer Token: {token}");

                    var response = await httpClient.PutAsJsonAsync(requestUrl, req);
                    Debug.WriteLine($"Response Status Code: {response.StatusCode}");

                    // Verificar el código de estado de la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Error: La solicitud falló con el código de estado {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Contenido de la respuesta de error: {errorContent}");
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar la información. Código de estado: {response.StatusCode}", "OK");
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
                        Debug.WriteLine($"Resultado de deserialización: {JsonConvert.SerializeObject(result)}");

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

                        // Mostrar confirmación de éxito
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Datos actualizados correctamente.", "OK");
                        Close();
                    }
                    catch (JsonException jsonEx)
                    {
                        Debug.WriteLine($"Error de deserialización: {jsonEx.Message}");
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
                    // Manejo de cualquier otro tipo de excepción
                    Debug.WriteLine($"Excepción inesperada: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
                }
            };
        }
    }




    private void Btcerrarsesion_Clicked(object sender, EventArgs e)
    {
        CerrarSesion();
    }

    private async void CerrarSesion()
    {

        Sesion.cerrarSesion();


        // Navegar de vuelta a la página de inicio de sesión
        await Navigation.PushAsync(new MainPage());

        // Mostrar un mensaje de confirmación
        await DisplayAlert("Cerrar sesión", "Has cerrado sesión correctamente.", "OK");
    }



    public class EliminarsesionPopup : Popup
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string laURL;

        public EliminarsesionPopup(string url)
        {
            laURL = url;

            var emailEntry = new Entry
            {
                Placeholder = "Ingrese su correo electrónico",
                Keyboard = Keyboard.Email,
                TextColor = Colors.Black
            };

            var enviarButton = new Button
            {
                Text = "Eliminar",
                BackgroundColor = Colors.Purple,
                TextColor = Colors.White
            };

            enviarButton.Clicked += async (sender, e) =>
            {
                var email = emailEntry.Text;

                if (string.IsNullOrEmpty(email))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El correo electrónico es obligatorio.", "OK");
                    return;
                }

                if (Sesion.id_usuario == null || Sesion.id_usuario <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se encontró un ID de usuario válido en la sesión.", "OK");
                    return;
                }

                // Eliminar la solicitud de cuerpo (req) para DELETE
                Debug.WriteLine($"Id_Usuario: {Sesion.id_usuario}, Correo Electrónico: {email}");

                try
                {
                    if (string.IsNullOrEmpty(laURL))
                    {
                        Debug.WriteLine("Error: La URL está vacía o no es válida.");
                        await Application.Current.MainPage.DisplayAlert("Error", "La URL no es válida.", "OK");
                        return;
                    }

                    var requestUrl = $"{laURL}/Usuarios/EliminarUsuario/{Sesion.id_usuario}?idUsuario={Sesion.id_usuario}&correoElectronico={Uri.EscapeDataString(email)}";
                    Debug.WriteLine($"Request URL: {requestUrl}");

                    // Añadir el token Bearer a los encabezados
                    string token = Sesion.token; // Asumiendo que el token está guardado en Sesion
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Debug.WriteLine($"Bearer Token: {token}");
                    Debug.WriteLine($"Authorization Header: {httpClient.DefaultRequestHeaders.Authorization}");

                    Debug.WriteLine("Enviando la solicitud DELETE...");
                    var response = await httpClient.DeleteAsync(requestUrl);
                    Debug.WriteLine($"Response Status Code: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Error: La solicitud falló con el código de estado {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Contenido de la respuesta de error: {errorContent}");
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar la información. Código de estado: {response.StatusCode}", "OK");
                        return;
                    }

                    // Leer el contenido de la respuesta
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response JSON: {jsonString}");

                    // Mostrar el contenido de la respuesta en una alerta
                    await Application.Current.MainPage.DisplayAlert("Éxito", jsonString, "OK");

                    Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Excepción inesperada: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
                }
            };

            Content = new StackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
            {
                emailEntry,
                enviarButton
            },
                WidthRequest = 300,
                HeightRequest = 200
            };
        }
    }

    private async void bteliminar_Clicked(object sender, EventArgs e)
    {
        var popup = new EliminarsesionPopup(LaURL); // Usa la URL definida en MainPage
        await Application.Current.MainPage.ShowPopupAsync(popup);
    }
}












