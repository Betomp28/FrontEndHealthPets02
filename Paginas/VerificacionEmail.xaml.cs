using FrontEndHealthPets.Services;
using FrontEndHealthPets.Paginas.FlyPaginas;

namespace FrontEndHealthPets.Paginas
{
    public partial class VerificacionEmail : ContentPage
    {
        private readonly string _email;
        private readonly IAuthenticationService _authService;
        private int _resendCountdown = 60;
        private bool _canResend = false;
        private readonly Entry[] _codeEntries;

        public VerificacionEmail(string email)
        {
            InitializeComponent();
            _email = email;
            _authService = new AuthenticationService();
            
            UserEmailLabel.Text = email;
            
            // Initialize code entries array for easy access
            _codeEntries = new Entry[] { Code1, Code2, Code3, Code4, Code5, Code6 };
            
            // Start countdown timer
            StartResendTimer();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Focus first code input
            Code1.Focus();
        }

        private void OnCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
            {
                // Auto-move to next input
                int currentIndex = Array.IndexOf(_codeEntries, entry);
                if (currentIndex < _codeEntries.Length - 1)
                {
                    _codeEntries[currentIndex + 1].Focus();
                }
                
                // Check if all codes are filled
                if (IsCodeComplete())
                {
                    // Auto-verify when all digits are entered
                    btVerificar_Clicked(this, EventArgs.Empty);
                }
            }
        }

        private bool IsCodeComplete()
        {
            foreach (var entry in _codeEntries)
            {
                if (string.IsNullOrEmpty(entry.Text))
                    return false;
            }
            return true;
        }

        private string GetFullCode()
        {
            return string.Join("", _codeEntries.Select(e => e.Text ?? ""));
        }

        private void ClearCode()
        {
            foreach (var entry in _codeEntries)
            {
                entry.Text = "";
            }
            Code1.Focus();
        }

        private async void btVerificar_Clicked(object sender, EventArgs e)
        {
            var code = GetFullCode();
            
            if (code.Length != 6)
            {
                ErrorLabel.Text = "Por favor ingresa el código completo de 6 dígitos";
                ErrorLabel.IsVisible = true;
                return;
            }

            ErrorLabel.IsVisible = false;
            LoadingOverlay.IsVisible = true;
            btVerificar.IsEnabled = false;

            try
            {
                var (success, error) = await _authService.VerifyEmailAsync(_email, code);

                if (success)
                {
                    LoadingOverlay.IsVisible = false;
                    SuccessOverlay.IsVisible = true;
                    
                    // Wait for animation then navigate
                    await Task.Delay(2000);
                    
                    // Navigate to main page
                    Application.Current!.MainPage = new NavigationPage(new PagFlyPrincipal());
                }
                else
                {
                    ErrorLabel.Text = error ?? "Código incorrecto. Intenta de nuevo.";
                    ErrorLabel.IsVisible = true;
                    ClearCode();
                }
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = $"Error de verificación: {ex.Message}";
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                btVerificar.IsEnabled = true;
            }
        }

        private async void OnResendTapped(object sender, EventArgs e)
        {
            if (!_canResend)
            {
                await DisplayAlert("Espera", $"Podrás reenviar el código en {_resendCountdown} segundos", "OK");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                // TODO: Call API to resend verification code
                await Task.Delay(1000); // Simulate API call
                
                await DisplayAlert("Código Enviado", 
                    $"Hemos enviado un nuevo código de verificación a {_email}", "OK");
                
                // Reset timer
                _resendCountdown = 60;
                _canResend = false;
                StartResendTimer();
                ClearCode();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo reenviar el código: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void StartResendTimer()
        {
            ResendLabel.TextColor = Color.FromArgb("#B2BEC3");
            TimerLabel.IsVisible = true;
            _canResend = false;

            while (_resendCountdown > 0)
            {
                TimerLabel.Text = $"({_resendCountdown}s)";
                await Task.Delay(1000);
                _resendCountdown--;
            }

            TimerLabel.IsVisible = false;
            ResendLabel.TextColor = Color.FromArgb("#6C63FF");
            _canResend = true;
        }

        private async void OnBackToLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
