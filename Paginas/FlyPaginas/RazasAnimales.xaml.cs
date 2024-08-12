using FrontEndHealthPets.Modelo;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class RazasAnimales : ContentPage
{
    private readonly DogApiService _dogViewModel;
    private Dictionary<string, List<string>> _breeds;

    public RazasAnimales()
	{
		InitializeComponent();
        _dogViewModel = new DogApiService(new HttpClient()); // Proporciona un HttpClient aquí
        LoadBreedsAsync();
    }

    private async Task LoadBreedsAsync()
    {
        try
        {
            // Obtener la lista de razas desde el servicio
            _breeds = await _dogViewModel.GetBreedsAsync();

            // Convertir las claves del diccionario (nombres de razas principales) en una lista
            var breedList = _breeds.Keys.ToList();

            // Verificar si la lista tiene elementos
            if (breedList.Count > 0)
            {
                // Asignar la lista al Picker
                dogSelector.ItemsSource = breedList;
            }
            else
            {
                // Mostrar mensaje si la lista está vacía
                await DisplayAlert("Información", "No se encontraron razas de perros.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción que ocurra durante la solicitud
            Console.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", $"No se pudo obtener las razas de perros: {ex.Message}", "OK");
        }
    }

    private async void BtBuscar_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener la raza seleccionada del Picker
            var selectedBreed = dogSelector.SelectedItem as string;

            if (selectedBreed != null)
            {
                // Obtener las imágenes correspondientes a la raza seleccionada
                var images = await _dogViewModel.GetImagesAsync(selectedBreed);

                // Aquí puedes agregar el código para mostrar las imágenes en la interfaz de usuario
                // Por ejemplo, podrías mostrarlas en una galería o en un control de imagen
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción que ocurra al intentar obtener imágenes de la raza seleccionada
            Console.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", "No se pudo procesar la raza seleccionada", "OK");
        }
    }
}

