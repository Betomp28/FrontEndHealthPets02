using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels
{
    public partial class VetProfileEditViewModel : ObservableObject
    {
        private readonly IVeterinarianService _veterinarianService;
        private readonly INavigationService _navigationService;
        private int _vetId;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string specialty;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string phone;

        [ObservableProperty]
        private string consultationPrice;

        [ObservableProperty]
        private string profileImageUrl;

        [ObservableProperty]
        private bool isLoading;

        public VetProfileEditViewModel(
            IVeterinarianService veterinarianService,
            INavigationService navigationService)
        {
            _veterinarianService = veterinarianService;
            _navigationService = navigationService;
        }

        public async Task<bool> InitializeAsync(int vetId)
        {
            _vetId = vetId;
            return await LoadProfileAndValidate();
        }

        private async Task<bool> LoadProfileAndValidate()
        {
            if (_vetId <= 0) return false;

            try
            {
                IsLoading = true;
                var vet = await _veterinarianService.GetVeterinarianByIdAsync(_vetId);
                
                if (vet != null)
                {
                    Name = vet.Name;
                    Specialty = vet.Specialty;
                    Description = vet.Description;
                    Phone = vet.Phone;
                    ConsultationPrice = vet.ConsultationPrice.ToString();
                    ProfileImageUrl = vet.ProfileImage;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading vet: {ex.Message}");
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadProfile()
        {
            await LoadProfileAndValidate();
        }

        [RelayCommand]
        private async Task SelectPhoto()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecciona tu foto de perfil"
                });

                if (result != null)
                {
                    IsLoading = true;

                    // Read file stream
                    using var stream = await result.OpenReadAsync();
                    
                    // Upload to server
                    var (success, imageUrl, error) = await _veterinarianService.UploadPhotoAsync(_vetId, stream, result.FileName);

                    if (success && !string.IsNullOrEmpty(imageUrl))
                    {
                        ProfileImageUrl = imageUrl;
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Foto actualizada", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", error ?? "No se pudo subir la foto", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al seleccionar foto: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El nombre es requerido", "OK");
                return;
            }

            try
            {
                IsLoading = true;

                decimal price = 0;
                decimal.TryParse(ConsultationPrice, out price);

                var (success, error) = await _veterinarianService.UpdateProfileAsync(
                    _vetId, Name, Specialty, Description, Phone, price);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Perfil actualizado", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", error ?? "No se pudo actualizar", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoToSchedule()
        {
            // Navigate to schedule management page with vetId
            var schedulePage = new FrontEndHealthPets.Paginas.VetScheduleManagementPage(_vetId);
            
            if (Application.Current?.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(schedulePage);
            }
        }
    }
}
