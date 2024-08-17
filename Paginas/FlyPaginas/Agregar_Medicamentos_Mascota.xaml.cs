using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.response;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;



namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class Agregar_Medicamentos_Mascota : ContentPage
    {
        private readonly HttpClient _httpClient;

        string LaURL = "https://localhost:44348/api";
        private ObservableCollection<Entidades.Medicamentos> Medicamentos { get; set; }

        public Agregar_Medicamentos_Mascota()
        {
            Debug.WriteLine("Constructor Agregar_Medicamentos_Mascota iniciado.");
            InitializeComponent();

            _httpClient = new HttpClient(); // Inicializa HttpClient
            Debug.WriteLine("HttpClient inicializado.");

            Medicamentos = new ObservableCollection<Entidades.Medicamentos>();
            Debug.WriteLine("ObservableCollection Medicamentos inicializado.");

            BindingContext = this;
            Debug.WriteLine("BindingContext establecido.");

            CargarMedicamentos();
            Debug.WriteLine("CargarMedicamentos llamado.");
        }

        private async void CargarMedicamentos()
        {
            Debug.WriteLine("Iniciando CargarMedicamentos.");
            var medicamentos = await ObtenerMedicamentosAsync();
            Debug.WriteLine($"Cantidad de medicamentos obtenidos: {medicamentos.Count}");

            Medicamentos.Clear(); // Limpia la colección antes de añadir nuevos medicamentos
            foreach (var medicamento in medicamentos)
            {
                Medicamentos.Add(medicamento);
                Debug.WriteLine($"Medicamento añadido: {medicamento.Nombre}");
            }

            // Actualiza el ItemsSource del Picker explícitamente
            MedicamentoPicker.ItemsSource = Medicamentos;
        }

        private async Task<List<Entidades.Medicamentos>> ObtenerMedicamentosAsync()
        {
            try
            {
                Debug.WriteLine($"Realizando solicitud GET a la API: {LaURL}/Lista_Medicamentos/Obtener_Lista_Medicamentos");

                // Crear la solicitud sin encabezado Content-Type
                var request = new HttpRequestMessage(HttpMethod.Get, LaURL + "/Lista_Medicamentos/Obtener_Lista_Medicamentos");


                // Enviar la solicitud
                var response = await _httpClient.SendAsync(request);
                Debug.WriteLine("Código de estado de la respuesta: " + response.StatusCode);

                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa
                Debug.WriteLine("Respuesta de la API fue exitosa.");

                // Leer la respuesta como una cadena
                var jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Respuesta de la API convertida a cadena.");

                // Deserializar la cadena JSON a un objeto Res_LIstaMedicamentos
                var result = JsonConvert.DeserializeObject<Res_LIstaMedicamentos>(jsonString);
                Debug.WriteLine($"Resultado de la deserialización: resultado={result.resultado}, Error={result.Error}");

                // Verificar si el resultado es exitoso y retornar la lista de medicamentos
                if (result.resultado)
                {
                    Debug.WriteLine("Resultado exitoso. Retornando lista de medicamentos.");
                    return result.ListarMedicamentos;
                }
                else
                {
                    Debug.WriteLine($"Error en la API: {result.Error}");
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Entidades.Medicamentos>(); // Retorna una lista vacía en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP
                Debug.WriteLine($"Excepción HttpRequestException: {ex.Message}");
                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
                return new List<Entidades.Medicamentos>(); // Retorna una lista vacía en caso de error
            }
        }

        private void MedicamentoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Evento SelectedIndexChanged del Picker activado.");

            // Verifica si hay algún elemento seleccionado
            if (MedicamentoPicker.SelectedIndex != -1)
            {
                var medicamentoSeleccionado = (Entidades.Medicamentos)MedicamentoPicker.SelectedItem;

                if (medicamentoSeleccionado != null)
                {
                    Debug.WriteLine($"Medicamento seleccionado: {medicamentoSeleccionado.Nombre}");
                    Debug.WriteLine($"Categoria: {medicamentoSeleccionado.Categoria}");
                    Debug.WriteLine($"Descripcion: {medicamentoSeleccionado.Decripcion}");
                    Debug.WriteLine($"Fecha de Vencimiento: {medicamentoSeleccionado.FechaDeVencimiento}");

                    // Aquí puedes actualizar tu BindingContext o realizar alguna acción adicional
                    BindingContext = medicamentoSeleccionado;
                }
                else
                {
                    Debug.WriteLine("No se ha seleccionado ningún medicamento.");
                }
            }
            else
            {
                Debug.WriteLine("No se ha seleccionado ningún índice en el Picker.");
            }
        }

    }
}
    
