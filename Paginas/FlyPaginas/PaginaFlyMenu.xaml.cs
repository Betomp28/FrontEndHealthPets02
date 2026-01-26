using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Paginas.FlyPaginas;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas
{
    public partial class PaginaFlyMenu : ContentPage
    {
        public PaginaFlyMenu()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            // Load user information from settings
            if (!string.IsNullOrEmpty(Settings.UserName))
            {
                UserNameLabel.Text = $"¡Hola, {Settings.UserName.Split(' ')[0]}!";
            }
            else
            {
                UserNameLabel.Text = "¡Hola, Usuario!";
            }

            if (!string.IsNullOrEmpty(Settings.UserEmail))
            {
                UserEmailLabel.Text = Settings.UserEmail;
            }
            else
            {
                UserEmailLabel.Text = "usuario@email.com";
            }


        }

        private async void OnDashboardTapped(object sender, EventArgs e)
        {
            try
            {
                await AnimateItem(sender as Frame);
                if (Parent is FlyoutPage flyout)
                {
                    var page = new FrontEndHealthPets.Pages.DashboardPage();
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OnDashboardTapped] Error: {ex.Message}");
                await DisplayAlert("Error", "No se pudo abrir el Dashboard", "OK");
            }
        }

        private async void OnMisMascotasTapped(object sender, EventArgs e)
        {
            try
            {
                await AnimateItem(sender as Frame);
                if (Parent is FlyoutPage flyout)
                {
                    var page = new MisMascotas();
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OnMisMascotasTapped] Error: {ex.Message}");
                await DisplayAlert("Error", "No se pudo abrir Mis Mascotas", "OK");
            }
        }

        private async void OnBathControlTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            
            if (Parent is FlyoutPage flyout)
            {
                var page = new FrontEndHealthPets.Pages.BathControlPage();
                flyout.Detail = new NavigationPage(page);
                try { flyout.IsPresented = false; } catch { }
            }
        }
        
        private async void OnVacunacionTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            
            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.VaccinationPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de vacunación", "OK");
                }
            }
        }

        private async void OnMedicamentosTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);

            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.MedicationPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de medicamentos", "OK");
                }
            }
        }

        private async void OnWeightTrackingTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);

            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.WeightTrackingPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de seguimiento de peso", "OK");
                }
            }
        }

        private async void OnVeterinariasTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            
            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.ClinicsPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de clínicas", "OK");
                }
            }
        }

        private async void OnVeterinariansListTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);

            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.VeterinariansPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de veterinarios", "OK");
                }
            }
        }

        private async void OnMyAppointmentsTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            // Since there is no route mapping for "MyAppointments" in NavigationService yet, we should add it or use direct navigation
            // Let's rely on NavigationService having logic, I'll update NavigationService next step to be sure
            // Actually I defined "VeterinariansList" in NavigationService but not "MyAppointments".
            // I'll update NavigationService to include "MyAppointments" mapping as well.
            // For now, let's assume I'll do that.
            var navService = ServiceHelper.GetService<INavigationService>();
            if (navService != null)
            {
                 // Temporary direct navigation if route not found logic isn't robust, but let's try route
                 // I will add "MyAppointments" route to NavigationService
                 await navService.NavigateToAsync("MyAppointments");
            }
        }

        private async void OnRecordatoriosTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            
            if (Parent is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.RecordatoriosPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                    try { flyout.IsPresented = false; } catch { }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la página de recordatorios", "OK");
                }
            }
        }



        private async void OnConfigTapped(object sender, EventArgs e)
        {
            await AnimateItem(sender as Frame);
            // For now, show a message - can implement settings page later
            await DisplayAlert("Próximamente", "La configuración estará disponible pronto", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro de que quieres cerrar sesión?",
                "Sí, cerrar sesión",
                "Cancelar"
            );

            if (confirm)
            {
                // Clear authentication data
                Settings.ClearAuth();
                
                // Navigate back to login page
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }

        private async Task AnimateItem(Frame? frame)
        {
            if (frame == null) return;
            
            await frame.ScaleTo(0.95, 100);
            await frame.ScaleTo(1.0, 100);
        }
    }
}