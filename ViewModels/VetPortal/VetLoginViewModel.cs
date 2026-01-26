using System.Windows.Input;
using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels.VetPortal
{
    public class VetLoginViewModel : BaseViewModel
    {
        private readonly IVetAuthenticationService _vetAuthService;
        private readonly INavigationService _navigationService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _emailError = string.Empty;
        private string _passwordError = string.Empty;

        public VetLoginViewModel(IVetAuthenticationService vetAuthService, INavigationService navigationService)
        {
            _vetAuthService = vetAuthService;
            _navigationService = navigationService;

            Title = "Portal Veterinario";

            LoginCommand = new Command(async () => await OnLoginAsync(), () => !IsBusy);
            BackToMainCommand = new Command(async () => await OnBackToMainAsync());
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
        public ICommand BackToMainCommand { get; }

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
                var (success, vet, error) = await _vetAuthService.LoginAsync(Email, Password);

                if (success && vet != null)
                {
                    // Correct navigation: Switch MainPage to Vet Portal
                    try 
                    {
                        var vetMainPage = ServiceHelper.GetService<Paginas.VetPortal.VetMainPage>();
                        if (vetMainPage != null)
                        {
                            Application.Current!.MainPage = vetMainPage;
                        }
                        else
                        {
                            await ShowAlert("Error", "No se pudo cargar el portal veterinario. Verifica la configuración.");
                        }
                    }
                    catch (Exception ex)
                    {
                        await ShowAlert("Error", $"Error al navegar: {ex.Message}");
                    }
                }
                else
                {
                    await ShowAlert("Error", error);
                }
            });
        }

        private async Task OnBackToMainAsync()
        {
            await _navigationService.NavigateToAsync("//Login");
        }
    }
}
