using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.response;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
namespace FrontEndHealthPets.Paginas.tabpage
{
    public partial class Consultas_Medicamentos : ContentPage
    {
        private readonly HttpClient _httpClient;
        private string LaURL = "https://localhost:44348/api";

        private ObservableCollection<Registro_Mascota> Mascotas { get; set; }
        private ObservableCollection<MedicamentosMascotas> MedicamentosMascotas { get; set; }

        public Consultas_Medicamentos()
        {
            InitializeComponent();
            _httpClient = new HttpClient(); // Inicializa HttpClient
            Mascotas = new ObservableCollection<Registro_Mascota>();
            MedicamentosMascotas = new ObservableCollection<MedicamentosMascotas>();
            BindingContext = this;

            // Cargar las mascotas al iniciar la página
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

            // Actualiza el ItemsSource del Picker explícitamente
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
                    Debug.WriteLine($"Error en la API: {result.Error}");
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
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
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
                return new List<Registro_Mascota>();
            }
        }

        // Maneja la selección de mascotas en el Picker
        private async void MascotaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MascotaPicker.SelectedIndex != -1)
            {
                var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;
                Debug.WriteLine($"Mascota seleccionada: {mascotaSeleccionada?.Nombre}");

                if (mascotaSeleccionada != null)
                {
                    // Cargar los medicamentos de la mascota seleccionada para mostrarlos en pantalla
                    await CargarMedicamentosMascota(mascotaSeleccionada.Id_Mascota);

                    // Actualiza el ItemsSource del ListView para mostrar los datos de medicamentos
                    MedicamentosListView.ItemsSource = MedicamentosMascotas;
                }
            }
        }

        // Carga de medicamentos de la mascota
        private async Task CargarMedicamentosMascota(int id_Mascota)
        {
            try
            {
                var requestUrl = $"{LaURL}/Lista_Medicamentos_Mascotas/Obtener_Lista_Medicamentos_Mascotas?id_mascota={id_Mascota}";
                Debug.WriteLine($"Request URL: {requestUrl}");

                var response = await _httpClient.GetAsync(requestUrl);
                Debug.WriteLine($"Response Status Code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response JSON: {jsonString}");

                var result = JsonConvert.DeserializeObject<Res_Lista_Medicamentos_Mascotas>(jsonString);

                if (result.resultado)
                {
                    MedicamentosMascotas.Clear(); // Limpiar la colección antes de agregar nuevos elementos

                    foreach (var medicamento in result.ListaMedicamentosMascotas)
                    {
                        Debug.WriteLine($"Medicamento: {medicamento.Nombre_Medicamento}, " +
                   $"Categoría: {medicamento.categoria}, " +
                   $"Modo de Administración: {medicamento.Modo_De_Administracion}, " +
                   $"Hora de Ingesta: {medicamento.Hora_De_Ingesta}, " +
                   $"Fecha de Inicio: {medicamento.Fecha_Inicio}, " +
                   $"Fecha de Fin: {medicamento.Fecha_Fin}, " +
                   $"Notas: {medicamento.Notas}");
                        MedicamentosMascotas.Add(medicamento);
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Error en la API: {result.Error ?? "Desconocido"}", "OK");
                }
                // Forzar la actualización del ListView (opcional)
               
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }
}
