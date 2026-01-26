using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class VeterinariansViewModel : ObservableObject
    {
        private readonly IVeterinarianService _veterinarianService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string searchText;

        public ObservableCollection<Veterinarian> Veterinarians { get; } = new();

        public VeterinariansViewModel(IVeterinarianService veterinarianService, INavigationService navigationService)
        {
            _veterinarianService = veterinarianService;
            _navigationService = navigationService;
            LoadVeterinariansCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadVeterinarians()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Veterinarians.Clear();
                
                var vets = await _veterinarianService.GetVeterinariansAsync(SearchText);
                
                foreach (var vet in vets)
                {
                    Veterinarians.Add(vet);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar los veterinarios", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Search()
        {
            await LoadVeterinarians();
        }

        [RelayCommand]
        private async Task GoToDetails(Veterinarian vet)
        {
            if (vet == null) return;

            try
            {
                // Get the profile page from DI and navigate
                var profileViewModel = Helpers.ServiceHelper.GetService<VeterinarianProfileViewModel>();
                if (profileViewModel != null)
                {
                    profileViewModel.VetId = vet.Id;
                    var profilePage = new FrontEndHealthPets.Paginas.VeterinarianProfilePage(profileViewModel);
                    
                    // Navigate using the current navigation stack
                    if (Application.Current?.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage navPage)
                    {
                        await navPage.PushAsync(profilePage);
                    }
                    else if (Application.Current?.MainPage is NavigationPage nav)
                    {
                        await nav.PushAsync(profilePage);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir el perfil: {ex.Message}", "OK");
            }
        }
    }
}
