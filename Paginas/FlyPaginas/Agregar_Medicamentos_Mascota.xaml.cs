using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.Entitys;
using FrontEndHealthPets.Entidades.response;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;


//todobien
namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class Agregar_Medicamentos_Mascota : ContentPage
    {
        private readonly HttpClient _httpClient;

        string LaURL = "https://localhost:44348/api";
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
        // cargar medicamentos


        // forzar el timepicker a que muestre la hora en formato de 24 horas
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


            Medicamentos.Clear(); // Limpia la colecci�n antes de a�adir nuevos medicamentos
            foreach (var medicamento in medicamentos)
            {
                Medicamentos.Add(medicamento);

            }

            // Actualiza el ItemsSource del Picker expl�citamente
            MedicamentoPicker.ItemsSource = Medicamentos;
        }

        // CArgar mascotas metodo
        private async void CargarMascotas()
        {
            int id_usuario = (int)Sesion.id_usuario;

            // Imprime el ID del usuario en la salida de depuraci�n
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


        //mobtener lista mascotas
        private async Task<List<Registro_Mascota>> ObtenerMascotasAsync(int id_usuario)
        {
            try
            {
                Debug.WriteLine($"ID del usuario: {id_usuario}");
                Debug.WriteLine($"URL de solicitud: {LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}");

                // Crear la solicitud con el ID de usuario como par�metro de consulta
                var requestUrl = $"{LaURL}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                var response = await _httpClient.SendAsync(request);


                response.EnsureSuccessStatusCode(); // Lanza una excepci�n si la respuesta no es exitosa


                // Leer la respuesta como una cadena
                var jsonString = await response.Content.ReadAsStringAsync();


                // Deserializar la cadena JSON a un objeto Res_LIstaMedicamentos
                var result = JsonConvert.DeserializeObject<Res_Lista_mascotas>(jsonString);


                // Verificar si el resultado es exitoso y retornar la lista de medicamentos
                if (result.resultado)
                {

                    return result.ListaMascotas;
                }
                else
                {

                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Registro_Mascota>(); // Retorna una lista vac�a en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP

                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
                return new List<Registro_Mascota>(); // Retorna una lista vac�a en caso de error
            }
        }

        // obtener lista medicamentos
        private async Task<List<Entidades.Medicamentos>> ObtenerMedicamentosAsync()
        {
            try
            {


                // Crear la solicitud sin encabezado Content-Type
                var request = new HttpRequestMessage(HttpMethod.Get, LaURL + "/Lista_Medicamentos/Obtener_Lista_Medicamentos");


                // Enviar la solicitud
                var response = await _httpClient.SendAsync(request);


                response.EnsureSuccessStatusCode(); // Lanza una excepci�n si la respuesta no es exitosa


                // Leer la respuesta como una cadena
                var jsonString = await response.Content.ReadAsStringAsync();


                // Deserializar la cadena JSON a un objeto Res_LIstaMedicamentos
                var result = JsonConvert.DeserializeObject<Res_LIstaMedicamentos>(jsonString);


                // Verificar si el resultado es exitoso y retornar la lista de medicamentos
                if (result.resultado)
                {

                    return result.ListarMedicamentos;
                }
                else
                {

                    await DisplayAlert("Error", $"Error en la API: {result.Error}", "OK");
                    return new List<Entidades.Medicamentos>(); // Retorna una lista vac�a en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP

                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
                return new List<Entidades.Medicamentos>(); // Retorna una lista vac�a en caso de error
            }
        }

        // verificador de mascota
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
                    Debug.WriteLine("Error: No se seleccion� ning�n medicamento.");
                    return;
                }

                if (MascotaPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Seleccione una mascota", "Aceptar");
                    Debug.WriteLine("Error: No se seleccion� ninguna mascota.");
                    return;
                }

                if (ModoDeAdministracionPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Seleccione un modo de administraci�n", "Aceptar");
                    Debug.WriteLine("Error: No se seleccion� un modo de administraci�n.");
                    return;
                }

                if (HoraDeIngestaTimePicker == null)
                {
                    await DisplayAlert("Error", "HoraDeIngestaTimePicker no est� inicializado correctamente.", "Aceptar");
                    Debug.WriteLine("Error: HoraDeIngestaTimePicker no est� inicializado.");
                    return;
                }
                else
                if (HoraDeIngestaTimePicker == null)
                {
                    await DisplayAlert("Error", "HoraDeIngestaTimePicker no est� inicializado correctamente.", "Aceptar");
                    Debug.WriteLine("Error: HoraDeIngestaTimePicker no est� inicializado.");
                    return;
                }
                else if (HoraDeIngestaTimePicker.Time == TimeSpan.Zero)
                {
                    await DisplayAlert("Error", "Seleccione una hora de ingesta v�lida.", "Aceptar");
                    Debug.WriteLine($"Hora seleccionada: {HoraDeIngestaTimePicker.Time}");
                    return;
                }

                if (FechaDeInicioDatePicker.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "Seleccione una fecha de inicio v�lida", "Aceptar");
                    Debug.WriteLine("Error: La fecha de inicio es anterior a la fecha actual.");
                    return;
                }

                if (FechaDeFinDatePicker.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "Seleccione una fecha de fin v�lida", "Aceptar");
                    Debug.WriteLine("Error: La fecha de fin es anterior a la fecha actual.");
                    return;
                }

                if (string.IsNullOrEmpty(NotasEditor.Text))
                {
                    await DisplayAlert("Error", "Ingrese notas", "Aceptar");
                    Debug.WriteLine("Error: El campo de notas est� vac�o.");
                    return;
                }

                Debug.WriteLine("Creando la solicitud Req_MedicamentosMascotas");

                Req_MedicamentosMascotas req = new Req_MedicamentosMascotas
                {
                    medicamentosMascotas = new MedicamentosMascotas
                    {
                        Id_Mascota = mascotaSeleccionada.Id_Mascota, // Utiliza la mascota seleccionada
                        id_medicamento = medicamentoSeleccionado.Id_Medicamento, // Utiliza el medicamento seleccionado
                        Modo_De_Administracion = ModoDeAdministracionPicker.SelectedItem.ToString(),
                        Fecha_Inicio = FechaDeInicioDatePicker.Date,
                        Fecha_Fin = FechaDeFinDatePicker.Date,
                        Hora_De_Ingesta = HoraDeIngestaTimePicker.Time,
                        Notas = NotasEditor.Text
                    }
                };

                Debug.WriteLine("Serializando la solicitud a JSON");

                var jsoncontent = new StringContent(JsonConvert.SerializeObject(req), System.Text.Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();

                Debug.WriteLine("Enviando la solicitud al servidor");

                var response = await httpClient.PostAsync(LaURL + "/Ingresar_Medicamentos_Mascotas/Ingresar_Medicamentos_Mascotas", jsoncontent);

                if (response.IsSuccessStatusCode) // saber si el API est� vivo
                {
                    Debug.WriteLine("Respuesta exitosa del servidor");

                    var responsecontent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Contenido de la respuesta: {responsecontent}");

                    Res_MedicamentosMascotas res = JsonConvert.DeserializeObject<Res_MedicamentosMascotas>(responsecontent);

                    if (res.resultado)
                    {
                        Debug.WriteLine("El medicamento se registr� correctamente");

                        await DisplayActionSheet("Registro", "Usuario Registrado", "Aceptar");
                        await Navigation.PushAsync(new MainPage());
                    }
                    else
                    {
                        Debug.WriteLine($"Error en el backend: {res.Error}");
                        await DisplayAlert("Error", res.Error, "Aceptar");
                    }
                }
                else
                {
                    Debug.WriteLine("Error de conexi�n: no se pudo conectar al servidor");
                    await DisplayAlert("Error de conexi�n", "Ocurri� un error de conexi�n", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excepci�n: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }
    }
}
