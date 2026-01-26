using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Paginas;
using FrontEndHealthPets.Paginas.FlyPaginas;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets
{
    public partial class MainPage : ContentPage
    {
        private readonly IAuthenticationService _authService;

        public MainPage()
        {
            InitializeComponent();
            _authService = new AuthenticationService();
            
            // Load saved credentials if present
            if (Settings.RememberMe && !string.IsNullOrEmpty(Settings.UserEmail))
            {
                Correo.Text = Settings.UserEmail;
                RememberMeCheckbox.IsChecked = true;
            }
        }

        private async void btiniciarsecion_Clicked(object sender, EventArgs e)
        {
            // Reset error messages
            EmailErrorLabel.IsVisible = false;
            PasswordErrorLabel.IsVisible = false;

            // Validate inputs
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(Correo.Text))
            {
                EmailErrorLabel.Text = "El email es requerido";
                EmailErrorLabel.IsVisible = true;
                isValid = false;
            }
            else if (!Validators.IsValidEmail(Correo.Text))
            {
                EmailErrorLabel.Text = "Email inválido";
                EmailErrorLabel.IsVisible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Passwoord.Text))
            {
                PasswordErrorLabel.Text = "La contraseña es requerida";
                PasswordErrorLabel.IsVisible = true;
                isValid = false;
            }
            else if (Passwoord.Text.Length < 6)
            {
                PasswordErrorLabel.Text = "La contraseña debe tener al menos 6 caracteres";
                PasswordErrorLabel.IsVisible = true;
                isValid = false;
            }

            if (!isValid)
                return;

            // Show loading
            LoadingOverlay.IsVisible = true;
            btiniciarsecion.IsEnabled = false;

            try
            {
                // Call real authentication service
                var (success, user, error) = await _authService.LoginAsync(Correo.Text.Trim(), Passwoord.Text);

                if (success && user != null)
                {
                    // Save settings
                    Settings.RememberMe = RememberMeCheckbox.IsChecked;
                    if (Settings.RememberMe)
                    {
                        Settings.UserEmail = Correo.Text;
                    }

                    // Navigate to main page
                    Application.Current!.MainPage = new PagFlyPrincipal();
                }
                else
                {
                    // Show error from server
                    await DisplayAlert("Error de Inicio de Sesión", error ?? "Credenciales inválidas", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo iniciar sesión: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                btiniciarsecion.IsEnabled = true;
            }
        }

        private async void btregistrarse_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registro());
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                var navigationService = ServiceHelper.GetService<INavigationService>();
                if (navigationService != null)
                {
                    await navigationService.NavigateToAsync("ForgotPassword");
                }
                else
                {
                    // Fallback if DI fails (shouldn't happen if MauiProgram is correct)
                    await Navigation.PushAsync(new ForgotPasswordPage(new ViewModels.ForgotPasswordViewModel(new AuthenticationService(), new NavigationService())));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar: {ex.Message}", "OK");
            }
        }

        private async void btnVetPortal_Clicked(object sender, EventArgs e)
        {
            try
            {
                var vetLoginPage = ServiceHelper.GetService<Paginas.VetPortal.VetLoginPage>();
                if (vetLoginPage != null)
                {
                    await Navigation.PushAsync(vetLoginPage);
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar el portal veterinario", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir portal: {ex.Message}", "OK");
            }
        }
    }
}
