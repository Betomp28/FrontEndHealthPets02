using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using FrontEndHealthPets.Modelos;
using Microsoft.Maui.Controls;
using FrontEndHealthPets.Paginas.FlyPaginas;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using FrontEndHealthPets.Entidades;
using FrontEndHealthPets.Entidades.response;
using FrontEndHealthPets.Entidades.Entitys;



namespace FrontEndHealthPets.Modelos
{
    public class MascotasViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PerfilMascota> perfilMascotas;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:44348/api";

        public ObservableCollection<PerfilMascota> PerfilMascotas
        {
            get => perfilMascotas;
            set
            {
                perfilMascotas = value;
                OnPropertyChanged(nameof(PerfilMascotas));
            }
        }

        public ICommand VerDetallesCommand { get; }

        public MascotasViewModel()
        {
            PerfilMascotas = new ObservableCollection<PerfilMascota>();
            VerDetallesCommand = new Command<PerfilMascota>(OnVerDetalles);

            _httpClient = new HttpClient(); // Inicializa HttpClient

            // Cargar las mascotas al iniciar el ViewModel
            CargarMascotasRegistradas();
        }

        public async void CargarMascotasRegistradas()
        {
            try
            {
                int id_usuario = (int)Sesion.id_usuario; // Suponiendo que tienes el id_usuario almacenado en la sesión
                var mascotas = await ObtenerMascotasDelUsuarioAsync(id_usuario);

                foreach (var mascota in mascotas)
                {
                    // Convierte el objeto de respuesta a PerfilMascota y agrégalo a la colección
                    var perfilMascota = new PerfilMascota
                    {
                        Name = mascota.Nombre,
                        Especie = mascota.especie,
                        Raza = mascota.raza,
                        Fecha_Nacimiento = mascota.Fecha_Nacimiento,
                        // Obtiene las fotos de la mascota
                        ImageSource = await ObtenerImagenDeMascotaAsync(mascota.Id_Mascota) 
                    };
                    PerfilMascotas.Add(perfilMascota);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar las mascotas: {ex.Message}");
            }
        }
        private async Task<ImageSource> ObtenerImagenDeMascotaAsync(int id_mascota)
        {
            try
            {
                var fotos = await ObtenerFotosDeMascotaAsync(id_mascota);
                var fotoPrincipal = fotos.FirstOrDefault(); // Asumiendo que quieres la primera foto

                if (fotoPrincipal != null)
                {
                    return ImageSource.FromStream(() => new MemoryStream(fotoPrincipal.Foto));
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener la imagen de la mascota: {ex.Message}");
                return null;
            }
        }
        private async Task<List<Registro_Mascota>> ObtenerMascotasDelUsuarioAsync(int id_usuario)
        {
            try
            {
                var requestUrl = $"{ApiUrl}/Lista_Mascotas/Obtener_Lista_Mascotas?id_usuario={id_usuario}";
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
                    Debug.WriteLine($"Error en la API: {result.error}");
                    return new List<Registro_Mascota>();
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HttpRequestException: {ex.Message}");
                return new List<Registro_Mascota>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return new List<Registro_Mascota>();
            }
        }

        private async void OnVerDetalles(PerfilMascota mascota)
        {
            if (mascota != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new DetallesMascotaPage(mascota));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        }


        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param </param>
        /// <returns></returns>

        private async Task<List<FotosMascota>> ObtenerFotosDeMascotaAsync(int id_mascota)
        {
            try
            {
                var requestUrl = $"{ApiUrl}/Lista_Fotos_Mascotas/Obtener_Lista_Fotos_Mascotas?id_mascota={id_mascota}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Res_Lista_Fotos>(jsonString);

                if (result.resultado)
                {
                    return result.Lista_Fotos; // Suponiendo que `ListaFotos` es una lista de `FotosMascota`
                }
                else
                {
                    Debug.WriteLine($"Error en la API: {result.error}");
                    return new List<FotosMascota>();
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HttpRequestException: {ex.Message}");
                return new List<FotosMascota>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return new List<FotosMascota>();
            }
        }


    }


}