using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys; // Puede ser redundante, revisa si esta importación es necesaria
using FrontEndHealthPets.Entidades.response;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class Agregar_Medicamentos_Mascota : ContentPage
    {
        private readonly HttpClient _httpClient;

        private const string LaURL = "https://localhost:44348/api";
        private ObservableCollection<Entidades.Medicamentos> Medicamentos { get; set; }
        private ObservableCollection<Registro_Mascota> Mascotas { get; set; }

        public Agregar_Medicamentos_Mascota()
        {
            InitializeComponent();
            _httpClient = new HttpClient(); // Inicializa HttpClient

            Medicamentos = new ObservableCollection<Entidades.Medicamentos>();
            Mascotas = new ObservableCollection<Registro_Mascota>();

            BindingContext = this;

            CargarMedicamentos();
            CargarMascotas();

            // Asociar el evento al TimePicker
            HoraDeIngestaTimePicker.PropertyChanged += HoraDeIngestaTimePicker_PropertyChanged;
        }

        // Forzar el TimePicker a que muestre la hora en formato de 24 horas
        private void HoraDeIngestaTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Time")
            {
                Debug.WriteLine($"Hora seleccionada: {HoraDeIngestaTimePicker.Time}");
            }
        }

        private async void CargarMedicamentos()
        {
            var medicamentos = await ObtenerMedicamentosAsync();
            Medicamentos.Clear(); // Limpia la colección antes de añadir nuevos medicamentos
            foreach (var medicamento in medicamentos)
            {
                Medicamentos.Add(medicamento);
            }

            // Actualiza el ItemsSource del Picker explícitamente
            MedicamentoPicker.ItemsSource = Medicamentos;
        }

        private async void CargarMascotas()
        {
            int id_usuario = (int)Sesion.id_usuario;

            // Imprime el ID del usuario en la salida de depuración
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

        // Obtener lista de mascotas
        private async Task<List<Registro_Mascota>> ObtenerMascotasAsync(int id_usuario)
        {
            try
            {
                Debug.WriteLine($"ID del usuario: {id_usuario}");
                Debug.WriteLine($"URL de solicitud: {LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}");

                // Crear la solicitud con el ID de usuario como parámetro de consulta
                var requestUrl = $"{LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                // Leer la respuesta como una cadena
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserializar la cadena JSON a un objeto Res_Lista_mascotas
                var result = JsonConvert.DeserializeObject<Res_Lista_mascotas>(jsonString);

                // Verificar si el resultado es exitoso y retornar la lista de mascotas
                if (result.resultado)
                {
                    return result.ListaMascotas;
                }
                else
                {
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Registro_Mascota>(); // Retorna una lista vacía en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP
                await DisplayAlert("Error", $"No se pudo obtener la lista de mascotas: {ex.Message}", "OK");
                return new List<Registro_Mascota>(); // Retorna una lista vacía en caso de error
            }
        }

        // Obtener lista de medicamentos
        private async Task<List<Entidades.Medicamentos>> ObtenerMedicamentosAsync()
        {
            try
            {
                // Crear la solicitud sin encabezado Content-Type
                var request = new HttpRequestMessage(HttpMethod.Get, LaURL + "/Lista_Medicamentos/Obtener_Lista_Medicamentos");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                // Leer la respuesta como una cadena
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserializar la cadena JSON a un objeto Res_ListaMedicamentos
                var result = JsonConvert.DeserializeObject<Res_ListaMedicamentos>(jsonString);

                // Verificar si el resultado es exitoso y retornar la lista de medicamentos
                if (result.resultado)
                {
                    return result.ListarMedicamentos;
                }
                else
                {
                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Entidades.Medicamentos>(); // Retorna una lista vacía en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP
                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
                return new List<Entidades.Medicamentos>(); // Retorna una lista vacía en caso de error
            }
        }

        // Verificador de medicamentos
        private void MascotaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MascotaPicker.SelectedIndex != -1)
            {
                var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;
                if (mascotaSeleccionada != null)
                {
                    EspecieLabel.Text = mascotaSeleccionada.especie;
                    RazaLabel.Text = mascotaSeleccionada.raza;
                }
            }
        }

        private void MedicamentoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MedicamentoPicker.SelectedIndex != -1)
            {
                var medicamentoSeleccionado = (Entidades.Medicamentos)MedicamentoPicker.SelectedItem;
                if (medicamentoSeleccionado != null)
                {
                    CategoriaLabel.Text = medicamentoSeleccionado.Categoria;
                    DescripcionLabel.Text = medicamentoSeleccionado.Decripcion;
                    FechaDeVencimientoLabel.Text = medicamentoSeleccionado.FechaDeVencimiento.ToString("dd/MM/yyyy");
                }
            }
        }

        private async void BtaplicarMedicamento_Clicked(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Iniciando BtaplicarMedicamento_Clicked");
                Debug.WriteLine($"Hora seleccionada: {HoraDeIngestaTimePicker.Time}");
                var medicamentoSeleccionado = (Entidades.Medicamentos)MedicamentoPicker.SelectedItem;
                var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;

                Debug.WriteLine($"Medicamento seleccionado: {medicamentoSeleccionado?.Nombre ?? "Ninguno"}");
                Debug.WriteLine($"Mascota seleccionada: {mascotaSeleccionada?.Nombre ?? "Ninguna"}");

                if (MedicamentoPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Seleccione un medicamento", "Aceptar");
                    Debug.WriteLine("Error: No se seleccionó ningún medicamento.");
                    return;
                }

                if (MascotaPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Seleccione una mascota", "Aceptar");
                    Debug.WriteLine("Error: No se seleccionó ninguna mascota.");
                    return;
                }

                if (ModoDeAdministracionPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Seleccione un modo de administración", "Aceptar");
                    Debug.WriteLine("Error: No se seleccionó un modo de administración.");
                    return;
                }

                if (HoraDeIngestaTimePicker == null)
                {
                    await DisplayAlert("Error", "HoraDeIngestaTimePicker no está inicializado correctamente.", "Aceptar");
                    Debug.WriteLine("Error: HoraDeIngestaTimePicker no está inicializado.");
                    return;
                }
                else if (HoraDeIngestaTimePicker.Time == TimeSpan.Zero)
                {
                    await DisplayAlert("Error", "Seleccione una hora de ingesta válida.", "Aceptar");
                    Debug.WriteLine($"Hora seleccionada: {HoraDeIngestaTimePicker.Time}");
                    return;
                }

                if (FechaDeInicioDatePicker.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "Seleccione una fecha de inicio válida", "Aceptar");
                    Debug.WriteLine("Error: La fecha de inicio es anterior a la fecha actual.");
                    return;
                }

                if (FechaDeFinDatePicker.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "Seleccione una fecha de fin válida", "Aceptar");
                    Debug.WriteLine("Error: La fecha de fin es anterior a la fecha actual.");
                    return;
                }

                if (FechaDeInicioDatePicker.Date > FechaDeFinDatePicker.Date)
                {
                    await DisplayAlert("Error", "La fecha de fin debe ser posterior a la fecha de inicio.", "Aceptar");
                    Debug.WriteLine("Error: La fecha de fin es anterior a la fecha de inicio.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NotasEditor.Text))
                {
                    await DisplayAlert("Error", "Ingrese las notas", "Aceptar");
                    Debug.WriteLine("Error: No se ingresaron notas.");
                    return;
                }

                // Datos del medicamento a agregar
                var medicamento = new MedicamentoAgregar
                {
                    IdMedicamento = medicamentoSeleccionado.Id_Medicamento,
                    IdMascota = mascotaSeleccionada.Id_Mascota,
                    ModoAdministracion = ModoDeAdministracionPicker.SelectedItem.ToString(),
                    HoraIngesta = HoraDeIngestaTimePicker.Time,
                    FechaInicio = FechaDeInicioDatePicker.Date,
                    FechaFin = FechaDeFinDatePicker.Date,
                    Notas = NotasEditor.Text
                };

                var jsonMedicamento = JsonConvert.SerializeObject(medicamento);
                var content = new StringContent(jsonMedicamento, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(LaURL + "/AplicarMedicamento/AgregarMedicamento", content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Medicamento agregado exitosamente", "Aceptar");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo agregar el medicamento", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al agregar el medicamento: {ex.Message}", "Aceptar");
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
