using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class MedicationViewModel : ObservableObject
    {
        private readonly IPetService _petService;
        private ObservableCollection<Medication> _allMedications;

        [ObservableProperty]
        private ObservableCollection<Medication> medications;

        [ObservableProperty]
        private ObservableCollection<Pet> pets;

        [ObservableProperty]
        private Pet selectedPet;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool hasNoMedications;

        public MedicationViewModel(IPetService petService)
        {
            _petService = petService;
            Medications = new ObservableCollection<Medication>();
            Pets = new ObservableCollection<Pet>();
            _allMedications = new ObservableCollection<Medication>();

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                var userPets = await _petService.GetPetsAsync();
                if (userPets != null && userPets.Count > 0)
                {
                    foreach (var pet in userPets)
                    {
                        Pets.Add(pet);
                    }
                    SelectedPet = Pets.First();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pets: {ex.Message}");
                // Mock pets for demo if service fails
                if (Pets.Count == 0)
                {
                    Pets.Add(new Pet { Id = 1, Name = "Max" });
                    Pets.Add(new Pet { Id = 2, Name = "Luna" });
                    SelectedPet = Pets.First();
                }
            }

            LoadMockMedications();
            FilterAndSortMedications();

            IsLoading = false;
        }

        partial void OnSelectedPetChanged(Pet value)
        {
            FilterAndSortMedications();
        }

        /// <summary>
        /// Filtra medicamentos por mascota y ordena: Activos primero, luego Finalizados
        /// </summary>
        private void FilterAndSortMedications()
        {
            if (SelectedPet == null)
            {
                Medications.Clear();
                HasNoMedications = true;
                return;
            }

            // Filtrar por mascota y ordenar: Activos primero, luego por fecha de fin descendente
            var filtered = _allMedications
                .Where(m => m.MascotaId == SelectedPet.Id)
                .OrderBy(m => m.Status) // Active (0) antes que Finished (1)
                .ThenByDescending(m => m.FechaFin)
                .ToList();

            Medications.Clear();
            foreach (var medication in filtered)
            {
                Medications.Add(medication);
            }

            HasNoMedications = Medications.Count == 0;
        }

        private void LoadMockMedications()
        {
            long pet1Id = Pets.FirstOrDefault()?.Id ?? 1;
            long pet2Id = Pets.Count > 1 ? Pets[1].Id : 2;

            // Medicamentos para Max (Pet 1)
            _allMedications.Add(new Medication
            {
                MedicamentoId = "med_001",
                Nombre = "Amoxicilina",
                Dosis = "500mg",
                Frecuencia = "Cada 12 horas",
                FechaInicio = DateTime.Today.AddDays(-3),
                FechaFin = DateTime.Today.AddDays(4),
                MascotaId = pet1Id,
                Veterinario = new PrescribingVet
                {
                    Id = "vet_55",
                    Nombre = "Dr. Roberto Martínez",
                    Clinica = "VetCenter"
                },
                InstruccionesAdicionales = "Dar con comida"
            });

            _allMedications.Add(new Medication
            {
                MedicamentoId = "med_002",
                Nombre = "Nexgard Spectra",
                Dosis = "1 pastilla",
                Frecuencia = "Cada 24 horas",
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(30),
                MascotaId = pet1Id,
                Veterinario = new PrescribingVet
                {
                    Id = "vet_55",
                    Nombre = "Dr. Roberto Martínez",
                    Clinica = "VetCenter"
                },
                InstruccionesAdicionales = "Antiparasitario mensual"
            });

            _allMedications.Add(new Medication
            {
                MedicamentoId = "med_003",
                Nombre = "Meloxicam",
                Dosis = "0.2mg/kg",
                Frecuencia = "Cada 24 horas",
                FechaInicio = DateTime.Today.AddDays(-15),
                FechaFin = DateTime.Today.AddDays(-8),
                MascotaId = pet1Id,
                Veterinario = new PrescribingVet
                {
                    Id = "vet_32",
                    Nombre = "Dra. Ana García",
                    Clinica = "Clínica Veterinaria Norte"
                },
                InstruccionesAdicionales = "Antiinflamatorio post-cirugía"
            });

            // Medicamentos para Luna (Pet 2)
            _allMedications.Add(new Medication
            {
                MedicamentoId = "med_004",
                Nombre = "Prednisona",
                Dosis = "5mg",
                Frecuencia = "Cada 12 horas",
                FechaInicio = DateTime.Today.AddDays(-2),
                FechaFin = DateTime.Today.AddDays(12),
                MascotaId = pet2Id,
                Veterinario = new PrescribingVet
                {
                    Id = "vet_55",
                    Nombre = "Dr. Roberto Martínez",
                    Clinica = "VetCenter"
                },
                InstruccionesAdicionales = "Reducir dosis gradualmente"
            });

            _allMedications.Add(new Medication
            {
                MedicamentoId = "med_005",
                Nombre = "Cefalexina",
                Dosis = "250mg",
                Frecuencia = "Cada 8 horas",
                FechaInicio = DateTime.Today.AddDays(-20),
                FechaFin = DateTime.Today.AddDays(-10),
                MascotaId = pet2Id,
                Veterinario = new PrescribingVet
                {
                    Id = "vet_32",
                    Nombre = "Dra. Ana García",
                    Clinica = "Clínica Veterinaria Norte"
                },
                InstruccionesAdicionales = "Completar ciclo completo de antibiótico"
            });
        }

        [RelayCommand]
        private async Task NavigateToDetails(Medication medication)
        {
            if (medication == null) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    var statusText = medication.Status == MedicationStatus.Active
                        ? $"En Tratamiento ({medication.DiasRestantes} días restantes)"
                        : "Finalizado";

                    var message = $"Medicamento: {medication.Nombre}\n" +
                                  $"Dosis: {medication.DosisFormateada}\n" +
                                  $"Estado: {statusText}\n" +
                                  $"Veterinario: {medication.Veterinario.Nombre}\n" +
                                  $"Instrucciones: {medication.InstruccionesAdicionales}";

                    await Application.Current.MainPage.DisplayAlert(
                        "Detalle de Medicamento",
                        message,
                        "OK");
                }
            });
        }

        [RelayCommand]
        private async Task AddMedication()
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Agregar Medicamento",
                    $"Agregar nueva receta para {SelectedPet?.Name}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            IsLoading = true;
            // Simular recarga de datos
            await Task.Delay(500);
            FilterAndSortMedications();
            IsLoading = false;
        }
    }
}
