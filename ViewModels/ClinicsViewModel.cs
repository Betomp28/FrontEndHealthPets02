using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class ClinicsViewModel : ObservableObject
    {
        private readonly IClinicService _clinicService;

        [ObservableProperty]
        private ObservableCollection<Clinic> clinics = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private Clinic? selectedClinic;

        private List<Clinic> _allClinics = new();

        public ClinicsViewModel(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [RelayCommand]
        private async Task LoadClinicsAsync()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;
                _allClinics = await _clinicService.GetClinicsAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading clinics: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudieron cargar las clínicas", "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadClinicsAsync();
        }

        [RelayCommand]
        private async Task SelectClinicAsync(Clinic clinic)
        {
            if (clinic == null) return;

            SelectedClinic = clinic;
            
            // Show clinic details
            string details = $"📍 {clinic.Address}\n" +
                           $"📞 {clinic.Phone}\n" +
                           (string.IsNullOrEmpty(clinic.Description) ? "" : $"\n{clinic.Description}");

            await Shell.Current.DisplayAlert(clinic.Name, details, "OK");
        }

        [RelayCommand]
        private async Task CallClinicAsync(Clinic clinic)
        {
            if (clinic == null || string.IsNullOrEmpty(clinic.Phone)) return;

            try
            {
                PhoneDialer.Default.Open(clinic.Phone);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo realizar la llamada: {ex.Message}", "OK");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Clinics = new ObservableCollection<Clinic>(_allClinics);
            }
            else
            {
                var filtered = _allClinics.Where(c =>
                    c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.VeterinarianName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                Clinics = new ObservableCollection<Clinic>(filtered);
            }
        }
    }
}
