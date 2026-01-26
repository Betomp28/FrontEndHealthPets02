using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels
{
    [QueryProperty(nameof(Email), "Email")]
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string code;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isLoading;

        public ResetPasswordViewModel(
            IAuthenticationService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return;
            }

            try
            {
                IsLoading = true;
                var (success, error) = await _authService.ResetPasswordAsync(Email, Code, NewPassword);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Contraseña restablecida correctamente. Inicia sesión con tu nueva contraseña.", "OK");
                    // Navigate back to Login (assumed to be the previous or root page, but ensuring we go back safely)
                    await _navigationService.GoBackAsync(); 
                    // Ideally navigate to "Login" explicity if GoBack doesn't guarantee Login, 
                    // but usually this flow starts from Login so GoBack is safe.
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", error ?? "No se pudo restablecer la contraseña. Verifica el código.", "OK");
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
    }
}
