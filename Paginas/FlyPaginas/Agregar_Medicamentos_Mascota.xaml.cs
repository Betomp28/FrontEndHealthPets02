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
            
        }
            // cargar medicamentos
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

        // CArgar mascotas metodo
        private async void CargarMascotas()
        {
            
            var mascotas = await ObtenerMascotasAsync();
            

            Mascotas.Clear();
            foreach (var mascota in mascotas)
            {
                Mascotas.Add(mascota);
              
            }

            // Actualiza el ItemsSource del Picker explícitamente
            MascotaPicker.ItemsSource = Mascotas;
        }

        //mobtener lista mascotas
        private async Task<List<Registro_Mascota>> ObtenerMascotasAsync()
        {
            try
            {
               

                // Crear la solicitud sin encabezado Content-Type
                var request = new HttpRequestMessage(HttpMethod.Get, LaURL + "/Lista_Mascotas/Obtener_Lista_Mascotas");


                // Enviar la solicitud
                var response = await _httpClient.SendAsync(request);
               

                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa
                

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
                    return new List<Entidades.Registro_Mascota>(); // Retorna una lista vacía en caso de error
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de solicitud HTTP
               
                await DisplayAlert("Error", $"No se pudo obtener la lista de medicamentos: {ex.Message}", "OK");
                return new List<Entidades.Registro_Mascota>(); // Retorna una lista vacía en caso de error
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
               

                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa
               

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
             // verificador de medicamentos
        private void MedicamentoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
           

            // Verifica si hay algún elemento seleccionado
            if (MedicamentoPicker.SelectedIndex != -1)
            {
                var medicamentoSeleccionado = (Entidades.Medicamentos)MedicamentoPicker.SelectedItem;

                if (medicamentoSeleccionado != null)
                {
                    

                    // Aquí puedes actualizar tu BindingContext o realizar alguna acción adicional
                    BindingContext = medicamentoSeleccionado;
                }
                              
            }
        }

        private void MascotaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            

            // Verifica si hay algún elemento seleccionado
            if (MascotaPicker.SelectedIndex != -1)
            {
                var mascotaSeleccionada = (Registro_Mascota)MascotaPicker.SelectedItem;

                if (mascotaSeleccionada != null)
                {
                    
                    

                    // Aquí puedes actualizar tu BindingContext o realizar alguna acción adicional
                    BindingContext = mascotaSeleccionada;
                }
               
            }

        }
    }
}
    
