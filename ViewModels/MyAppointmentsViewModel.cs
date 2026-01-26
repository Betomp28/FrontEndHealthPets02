using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class MyAppointmentsViewModel : ObservableObject
    {
        private readonly IVeterinarianService _veterinarianService;

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<VetAppointment> Appointments { get; } = new();

        public MyAppointmentsViewModel(IVeterinarianService veterinarianService)
        {
            _veterinarianService = veterinarianService;
            LoadAppointmentsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadAppointments()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Appointments.Clear();
                
                var appointments = await _veterinarianService.GetMyAppointmentsAsync();
                
                foreach (var app in appointments)
                {
                    Appointments.Add(app);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar las citas", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelAppointment(VetAppointment appointment)
        {
            if (appointment == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Deseas cancelar esta cita?", "Sí", "No");
            if (!confirm) return;

            try
            {
                IsLoading = true;
                var (success, message) = await _veterinarianService.CancelAppointmentAsync(appointment.Id);
                
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", message, "OK");
                    await LoadAppointments(); // Reload list
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
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoToVeterinarians()
        {
            try
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.VeterinariansPage>();
                if (page != null && Application.Current?.MainPage is FlyoutPage flyout)
                {
                    flyout.Detail = new NavigationPage(page);
                    flyout.IsPresented = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GoToVeterinarians] Error: {ex.Message}");
            }
        }
    }
}
