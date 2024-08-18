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

        // carga de mascota

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
        // con esto obtenemos la lista de mascotas del usuario conectado
        private async Task<List<Registro_Mascota>> ObtenerMascotasAsync(int id_usuario)
        {
            try
            {
                var requestUrl = $"{LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Res_Lista_mascotas>(jsonString);

                if (result.resultado)
                {
                    return result.ListaMascotas;
                }
                else
                {
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Registro_Mascota>();
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error", $"No se pudo obtener la lista de mascotas: {ex.Message}", "OK");
                return new List<Registro_Mascota>();
            }
        }

        // pertenece a la carga de mascota
        private async void MascotaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MascotaPicker.SelectedIndex != -1)
            {
                var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;
                if (mascotaSeleccionada != null)
                {
                    // Cargar los medicamentos de la mascota seleccionada para mostrarlos en pantalla
                    await CargarMedicamentosMascota(mascotaSeleccionada.Id_Mascota);
                }
            }
        }

        // carga de medicamentos de la mascota datos cargarlos aqui
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

                MedicamentosMascotas.Clear();

                if (result.resultado)
                {
                    foreach (var medicamento in result.ListaMedicamentosMascotas)
                    {
                        MedicamentosMascotas.Add(medicamento);
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                }
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
