using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrontEndHealthPets.ViewModels
{
    public partial class VaccinationViewModel : ObservableObject
    {
        private readonly IPetService _petService;
        private ObservableCollection<Vaccine> _allVaccines;

        [ObservableProperty]
        private ObservableCollection<Vaccine> vaccines;

        [ObservableProperty]
        private ObservableCollection<Pet> pets;

        [ObservableProperty]
        private Pet selectedPet;

        public VaccinationViewModel(IPetService petService)
        {
            _petService = petService;
            Vaccines = new ObservableCollection<Vaccine>();
            Pets = new ObservableCollection<Pet>();
            _allVaccines = new ObservableCollection<Vaccine>();
            
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            // Load Pets
            try
            {
                var userPets = await _petService.GetPetsAsync();
                if (userPets != null && userPets.Count > 0)
                {
                    foreach (var pet in userPets)
                    {
                        Pets.Add(pet);
                    }
                    // Select first pet by default
                    SelectedPet = Pets.First();
                }
            }
            catch (Exception ex)
            {
                 // Handle error or use mock pets if service fails for demo
                 Console.WriteLine($"Error loading pets: {ex.Message}");
                 // Mock pets for demo purposes if backend fails or returns empty
                 if (Pets.Count == 0)
                 {
                     Pets.Add(new Pet { Id = 1, Name = "Max" });
                     Pets.Add(new Pet { Id = 2, Name = "Luna" });
                     SelectedPet = Pets.First();
                 }
            }

            LoadMockVaccines();
            FilterVaccines();
        }

        partial void OnSelectedPetChanged(Pet value)
        {
            FilterVaccines();
        }

        private void FilterVaccines()
        {
            if (SelectedPet == null)
            {
                Vaccines.Clear();
                return;
            }

            var filtered = _allVaccines.Where(v => v.PetId == SelectedPet.Id).ToList();
            Vaccines.Clear();
            foreach(var v in filtered)
            {
                Vaccines.Add(v);
            }
        }

        private void LoadMockVaccines()
        {
            // Add some mock data for different pets
            // Assumption: Pet IDs are 1 and 2 roughly based on mock pets created above
            // In a real scenario, this would come from an API endpoint like GetVaccinesByPetId
            
            long pet1Id = Pets.FirstOrDefault()?.Id ?? 1;
            long pet2Id = Pets.Count > 1 ? Pets[1].Id : 2;

            _allVaccines.Add(new Vaccine 
            { 
                PetId = pet1Id,
                Name = "Rabia", 
                DateAdministered = DateTime.Now.AddMonths(-11), 
                NextDueDate = DateTime.Now.AddMonths(1), 
                Status = VaccineStatus.Upcoming 
            });

            _allVaccines.Add(new Vaccine 
            { 
                PetId = pet1Id,
                Name = "Parvovirus", 
                DateAdministered = DateTime.Now.AddMonths(-13), 
                NextDueDate = DateTime.Now.AddMonths(-1), 
                Status = VaccineStatus.Overdue 
            });

            _allVaccines.Add(new Vaccine 
            { 
                PetId = pet2Id, // Second pet
                Name = "Moquillo", 
                DateAdministered = DateTime.Now.AddMonths(-2), 
                NextDueDate = DateTime.Now.AddMonths(10), 
                Status = VaccineStatus.Completed 
            });
            
             _allVaccines.Add(new Vaccine 
            { 
                PetId = pet2Id, // Second pet
                Name = "Rabia", 
                DateAdministered = DateTime.Now.AddMonths(-6), 
                NextDueDate = DateTime.Now.AddMonths(6), 
                Status = VaccineStatus.Upcoming 
            });
        }

        [RelayCommand]
        private async Task NavigateToDetails(Vaccine vaccine)
        {
            if (vaccine == null) return;
            // Use MainThread to ensure UI operations are safe
            await MainThread.InvokeOnMainThreadAsync(async () => 
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Detalle de Vacuna", $"Vacuna: {vaccine.Name}\nEstado: {vaccine.Status}\nMascota: {SelectedPet?.Name}", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task AddVaccine()
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Agregar Vacuna", $"Agregar nueva vacuna para {SelectedPet?.Name}", "OK");
            }
        }
    }
}
