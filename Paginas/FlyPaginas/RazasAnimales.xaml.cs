using FrontEndHealthPets.Modelo;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class RazasAnimales : ContentPage
{
    private readonly DogApiService _dogViewModel;
    private Dictionary<string, List<string>> _breeds;

    public RazasAnimales()
	{
		InitializeComponent();
        _dogViewModel = new DogApiService(new HttpClient()); // Proporciona un HttpClient aqu�
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
                // Mostrar mensaje si la lista est� vac�a
                await DisplayAlert("Informaci�n", "No se encontraron razas de perros.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepci�n que ocurra durante la solicitud
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
                // Obtener las im�genes correspondientes a la raza seleccionada
                var images = await _dogViewModel.GetImagesAsync(selectedBreed);

                // Aqu� puedes agregar el c�digo para mostrar las im�genes en la interfaz de usuario
                // Por ejemplo, podr�as mostrarlas en una galer�a o en un control de imagen
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepci�n que ocurra al intentar obtener im�genes de la raza seleccionada
            Console.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", "No se pudo procesar la raza seleccionada", "OK");
        }
    }
}

