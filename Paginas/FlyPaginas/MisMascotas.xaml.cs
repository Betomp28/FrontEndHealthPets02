using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class MisMascotas : ContentPage
{
    private IPetService? _petService;

    public MisMascotas()
    {
        try
        {
            InitializeComponent();
            _petService = ServiceHelper.GetService<IPetService>() ?? new PetService();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] Constructor error: {ex.Message}");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await LoadPetsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] OnAppearing error: {ex.Message}");
        }
    }

    private async Task LoadPetsAsync()
    {
        try
        {
            if (LoadingOverlay != null) LoadingOverlay.IsVisible = true;
            if (PetsScrollView != null) PetsScrollView.IsVisible = false;
            if (EmptyState != null) EmptyState.IsVisible = false;

            if (_petService == null)
            {
                _petService = new PetService();
            }

            var pets = await _petService.GetPetsAsync();

            if (pets == null || pets.Count == 0)
            {
                if (EmptyState != null) EmptyState.IsVisible = true;
                if (PetsScrollView != null) PetsScrollView.IsVisible = false;
            }
            else
            {
                // Create display models with age
                var displayPets = pets.Select(p => new PetDisplayModel
                {
                    Id = p.Id,
                    Name = p.Name ?? "",
                    Species = p.Species ?? "",
                    Breed = p.Breed ?? "Sin raza",
                    ProfileImage = p.PhotoUrl,
                    AgeDisplay = CalculateAge(p.BirthDate)
                }).ToList();

                if (PetsCollectionView != null) PetsCollectionView.ItemsSource = displayPets;
                if (PetsCount != null) PetsCount.Text = pets.Count.ToString();
                
                if (EmptyState != null) EmptyState.IsVisible = false;
                if (PetsScrollView != null) PetsScrollView.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] LoadPetsAsync error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] Stack trace: {ex.StackTrace}");
            
            try
            {
                await DisplayAlert("Error", $"Error cargando mascotas: {ex.Message}", "OK");
            }
            catch { }
            
            if (EmptyState != null) EmptyState.IsVisible = true;
            if (PetsScrollView != null) PetsScrollView.IsVisible = false;
        }
        finally
        {
            if (LoadingOverlay != null) LoadingOverlay.IsVisible = false;
        }
    }

    private string CalculateAge(DateTime birthDate)
    {
        if (birthDate == default) return "Edad desconocida";
        
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        
        if (age < 1)
        {
            var months = (today.Year - birthDate.Year) * 12 + today.Month - birthDate.Month;
            return $"{months} meses";
        }
        return age == 1 ? "1 año" : $"{age} años";
    }

    private async void btNuevaMascota_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new IngresarMascotas());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] Navigation error: {ex.Message}");
        }
    }

    private async void OnPetTapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (e.Parameter is PetDisplayModel petDisplay)
            {
                var pet = new Pet
                {
                    Id = petDisplay.Id,
                    Name = petDisplay.Name,
                    Species = petDisplay.Species,
                    Breed = petDisplay.Breed,
                    PhotoUrl = petDisplay.ProfileImage ?? ""
                };
                
                await Navigation.PushAsync(new FrontEndHealthPets.Pages.PetDetailPage(pet));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MisMascotas] OnPetTapped error: {ex.Message}");
            await DisplayAlert("Error", "No se pudo abrir el detalle de la mascota", "OK");
        }
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        // TODO: Implement search filter
    }
}

// Helper class for display
public class PetDisplayModel
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public string Breed { get; set; } = "";
    public string? ProfileImage { get; set; }
    public string AgeDisplay { get; set; } = "";
}