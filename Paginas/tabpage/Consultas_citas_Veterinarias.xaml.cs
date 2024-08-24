using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.response;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FrontEndHealthPets.Paginas.tabpage;

public partial class Consultas_citas_Veterinarias : ContentPage
{
    private readonly HttpClient _httpClient;
    private string LaURL = "https://localhost:44348/api";

    private ObservableCollection<Registro_Mascota> Mascotas { get; set; }
    private ObservableCollection<Citas_Clinica_Veterinaria_Mascotas> CitaClinicasVeterinaria { get; set; }
    public Consultas_citas_Veterinarias()
    {
        InitializeComponent();
        _httpClient = new HttpClient(); // Inicializa HttpClient
        Mascotas = new ObservableCollection<Registro_Mascota>();
        CitaClinicasVeterinaria = new ObservableCollection<Citas_Clinica_Veterinaria_Mascotas>();
        BindingContext = this;
        // Cargar las mascotas al iniciar la p�gina
        CargarMascotas();
    }

    // Carga de mascotas
    private async void CargarMascotas()
    {
        int id_usuario = (int)Sesion.id_usuario;
        Debug.WriteLine($"ID del usuario: {id_usuario}");

        var mascotas = await ObtenerMascotasAsync(id_usuario);
        Mascotas.Clear();
        foreach (var mascota in mascotas)
        {
            Mascotas.Add(mascota);
        }

        // Actualiza el ItemsSource del Picker expl�citamente
        MascotaPicker.ItemsSource = Mascotas;
    }

    // Obtener lista de mascotas del usuario conectado
    private async Task<List<Registro_Mascota>> ObtenerMascotasAsync(int id_usuario)
    {
        try
        {
            var requestUrl = $"{LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}";
            Debug.WriteLine($"Request URL: {requestUrl}");

            var response = await _httpClient.GetAsync(requestUrl);
            Debug.WriteLine($"Response Status Code: {response.StatusCode}");

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response JSON: {jsonString}");

            var result = JsonConvert.DeserializeObject<Res_Lista_mascotas>(jsonString);

            if (result.resultado)
            {
                Debug.WriteLine("Mascotas obtenidas correctamente.");
                return result.ListaMascotas;
            }
            else
            {
                Debug.WriteLine($"Error en la API: {result.error}");
                await DisplayAlert("Error", $"Error en la API: {result.error}", "OK");
                return new List<Registro_Mascota>();
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"HttpRequestException: {ex.Message}");
            await DisplayAlert("Error", $"No se pudo obtener la lista de mascotas: {ex.Message}", "OK");
            return new List<Registro_Mascota>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
            return new List<Registro_Mascota>();
        }
    }

    private async void MascotaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MascotaPicker.SelectedIndex != -1)
        {
            var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;
            Debug.WriteLine($"Mascota seleccionada: {mascotaSeleccionada?.Nombre}");

            if (mascotaSeleccionada != null)
            {
                
                await CargarCitasVeteriaMascota(mascotaSeleccionada.Id_Mascota );

                // Actualiza el ItemsSource del ListView para mostrar los datos de Citasclinicaveterinaria
                CitasClinicamascotasListView.ItemsSource = CitaClinicasVeterinaria;
            }
        }
    }
    //
    // Carga de medicamentos de la mascota
    private async Task CargarCitasVeteriaMascota(int id_Mascota )
    {
        try
        {
            // M�todo para obtener el id_doctor
            var requestUrl = $"{LaURL}/Lista_Citas_Veterinarias/Obtener_Lista_Citas_Veterinarias?id_mascota={id_Mascota}";
            Debug.WriteLine($"Request URL: {requestUrl}");

            var response = await _httpClient.GetAsync(requestUrl);
            Debug.WriteLine($"Response Status Code: {response.StatusCode}");

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response JSON: {jsonString}");
            Debug.WriteLine("jsonString: " + jsonString);
            var result = JsonConvert.DeserializeObject<Res_Lista_Citas_Veterinaria_mascotas>(jsonString);

            if (result.resultado)
            {
                CitaClinicasVeterinaria.Clear(); // Limpiar la colecci�n antes de agregar nuevos elementos

                foreach (var CitasVeterinarias in result.Lista_Citas_Veterinaria_Mascotas)
                {

                    Debug.WriteLine("Detalles de la cita veterinaria:");
                    Debug.WriteLine($"Nombre_Clinica: {CitasVeterinarias.Nombre_Clinica}");
                    Debug.WriteLine($"Direccion: {CitasVeterinarias.Direccion}");
                    Debug.WriteLine($"Telefono: {CitasVeterinarias.Telefono}");
                    Debug.WriteLine($"Nombre_Doctor: {CitasVeterinarias.Nombre_Doctor}");
                    Debug.WriteLine($"Fecha_y_hora_Cita: {CitasVeterinarias.Fecha_y_hora_Cita}");
                    Debug.WriteLine($"Notas: {CitasVeterinarias.Notas}");
                    Debug.WriteLine($"Nombre_Clinica: {CitasVeterinarias.Nombre_Clinica}, " +
               $"Nombre_Clinica:  {CitasVeterinarias.Direccion} " +
               $"Telefono: {CitasVeterinarias.Telefono}, " +
               $"Nombre_Doctor: {CitasVeterinarias.Nombre_Doctor}, " +
               $"Fecha_y_hora_Cita: {CitasVeterinarias.Fecha_y_hora_Cita}, " +
               $"Notas: {CitasVeterinarias.Notas}");
                    CitaClinicasVeterinaria.Add(CitasVeterinarias);
                }
            }
            else
            {
                await DisplayAlert("Error", $"Error en la API: {result.error ?? "Desconocido"}", "OK");
            }
            // Forzar la actualizaci�n del ListView (opcional)

        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
        }
    }
}


  