using System.Windows.Input;
using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _rememberMe = false;
        private string _emailError = string.Empty;
        private string _passwordError = string.Empty;

        public LoginViewModel(IAuthenticationService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            
            Title = "Bienvenido";

            LoginCommand = new Command(async () => await OnLoginAsync(), () => !IsBusy);
            NavigateToRegisterCommand = new Command(async () => await OnNavigateToRegisterAsync());
            ForgotPasswordCommand = new Command(async () => await OnForgotPasswordAsync());

            // Load saved email if remember me was enabled
            if (Settings.RememberMe)
            {
                Email = Settings.UserEmail;
                RememberMe = true;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateEmail();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidatePassword();
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public string EmailError
        {
            get => _emailError;
            set => SetProperty(ref _emailError, value);
        }

        public string PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }

        public bool HasEmailError => !string.IsNullOrEmpty(EmailError);
        public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = string.Empty;
                return false;
            }

            if (!Validators.IsValidEmail(Email))
            {
                EmailError = "Email inválido";
                return false;
            }

            EmailError = string.Empty;
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = string.Empty;
                return false;
            }

            if (Password.Length < 6)
            {
                PasswordError = "Mínimo 6 caracteres";
                return false;
            }

            PasswordError = string.Empty;
            return true;
        }

        private bool ValidateForm()
        {
            bool isEmailValid = ValidateEmail();
            bool isPasswordValid = ValidatePassword();

            if (!isEmailValid || !isPasswordValid)
            {
                if (!isEmailValid)
                    EmailError = "Email requerido";
                if (!isPasswordValid)
                    PasswordError = "Contraseña requerida";
                
                return false;
            }

            return true;
        }

        private async Task OnLoginAsync()
        {
            if (!ValidateForm())
                return;

            await ExecuteWithLoading(async () =>
            {
                var (success, user, error) = await _authService.LoginAsync(Email, Password);

                if (success && user != null)
                {
                    // Save remember me preference
                    Settings.RememberMe = RememberMe;
                    
                    // Navigate to main page
                    await _navigationService.NavigateToAsync("//Dashboard");
                }
                else
                {
                    await ShowAlert("Error", error);
                }
            });
        }

        private async Task OnNavigateToRegisterAsync()
        {
            await _navigationService.NavigateToAsync("Register");
        }

        private async Task OnForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await ShowAlert("Recuperar Contraseña", "Por favor ingresa tu email primero");
                return;
            }

            if (!Validators.IsValidEmail(Email))
            {
                await ShowAlert("Recuperar Contraseña", "Por favor ingresa un email válido");
                return;
            }

            bool confirm = await ShowConfirmation(
                "Recuperar Contraseña",
                $"¿Enviar email de recuperación a {Email}?",
                "Enviar",
                "Cancelar"
            );

            if (confirm)
            {
                await ExecuteWithLoading(async () =>
                {
                    var (success, error) = await _authService.RequestPasswordResetAsync(Email);

                    if (success)
                    {
                        await ShowAlert("Éxito", "Se ha enviado un email con instrucciones para recuperar tu contraseña");
                    }
                    else
                    {
                        await ShowAlert("Error", error);
                    }
                });
            }
        }
    }
}
