using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private bool isLoading;

        public ForgotPasswordViewModel(
            IAuthenticationService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task SendCode()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor ingresa tu correo electrónico", "OK");
                return;
            }

            try
            {
                IsLoading = true;
                var (success, error) = await _authService.RequestPasswordResetAsync(Email);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Código enviado. Revisa tu correo.", "OK");
                    // Navigate to Reset Password page, passing the email
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "Email", Email }
                    };
                    await _navigationService.NavigateToAsync("ResetPassword", navigationParameter);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", error ?? "No se pudo enviar el código. Verifica el correo.", "OK");
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
        private async Task GoBack()
        {
            await _navigationService.GoBackAsync();
        }
    }
}
