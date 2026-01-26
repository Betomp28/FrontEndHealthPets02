using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    [QueryProperty(nameof(VetId), "id")]
    public partial class VeterinarianProfileViewModel : ObservableObject
    {
        private readonly IVeterinarianService _veterinarianService;
        private readonly INavigationService _navigationService;
        private readonly IPetService _petService; // assuming we need to pick a pet

        [ObservableProperty]
        private int vetId;

        [ObservableProperty]
        private Veterinarian veterinarian;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private AvailableSlot selectedSlot;

        [ObservableProperty]
        private Pet selectedPet;

        [ObservableProperty]
        private string reason;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isBookLoading;

        public ObservableCollection<AvailableSlot> AvailableSlots { get; } = new();
        public ObservableCollection<Pet> MyPets { get; } = new();

        public VeterinarianProfileViewModel(
            IVeterinarianService veterinarianService, 
            INavigationService navigationService,
            IPetService petService)
        {
            _veterinarianService = veterinarianService;
            _navigationService = navigationService;
            _petService = petService;
        }

        async partial void OnVetIdChanged(int value)
        {
            await LoadData();
        }

        async partial void OnSelectedDateChanged(DateTime value)
        {
            await LoadAvailability();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Veterinarian = await _veterinarianService.GetVeterinarianByIdAsync(VetId);
                
                // Load user's pets for booking
                var pets = await _petService.GetPetsAsync();
                MyPets.Clear();
                foreach (var pet in pets)
                {
                    MyPets.Add(pet);
                }

                await LoadAvailability();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading vet profile: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadAvailability()
        {
            try
            {
                AvailableSlots.Clear();
                SelectedSlot = null; // Reset selection

                var slots = await _veterinarianService.GetAvailabilityAsync(VetId, SelectedDate);
                foreach (var slot in slots)
                {
                    AvailableSlots.Add(slot);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading availability: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task BookAppointment()
        {
            if (SelectedSlot == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Selecciona una hora", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Reason))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ingresa un motivo para la consulta", "OK");
                return;
            }

            try
            {
                IsBookLoading = true;
                
                var result = await _veterinarianService.BookAppointmentAsync(
                    VetId, 
                    SelectedSlot.DateTime, 
                    Reason, 
                    Notes, 
                    (int?)SelectedPet?.Id
                );

                var success = result.success;
                var message = result.message;

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", message, "OK");
                    await _navigationService.GoBackAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                IsBookLoading = false;
            }
        }
    }
}
