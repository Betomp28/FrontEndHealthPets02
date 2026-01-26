using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas
{
    public partial class Registro : ContentPage
    {
        private readonly IAuthenticationService _authService;

        public Registro()
        {
            InitializeComponent();
            _authService = new AuthenticationService();
        }

        private async void btRegistrar_Clicked(object sender, EventArgs e)
        {
            if (IsBusy) return;

            if (!ValidateForm())
                return;

            try
            {
                IsBusy = true;
                LoadingOverlay.IsVisible = true;

                // Call Register
                var result = await _authService.RegisterAsync(
                    Nombre.Text.Trim(),
                    Apellido.Text.Trim(),
                    Email.Text.Trim(),
                    Password.Text
                );

                LoadingOverlay.IsVisible = false;
                IsBusy = false;

                if (result.success)
                {
                    // Navigate to email verification page
                    await DisplayAlert("¡Casi listo!", 
                        "Hemos enviado un código de verificación a tu correo electrónico. Por favor verifica tu identidad para completar el registro.", 
                        "Continuar");
                    
                    await Navigation.PushAsync(new VerificacionEmail(Email.Text.Trim()));
                }
                else
                {
                    await DisplayAlert("Error", result.error ?? "Error al registrar", "OK");
                }
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                IsBusy = false;
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        private async void btRegresar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private bool ValidateForm()
        {
            // Reset errors
            NombreError.IsVisible = false;
            ApellidoError.IsVisible = false;
            EmailError.IsVisible = false;
            ConfirmEmailError.IsVisible = false;
            PasswordError.IsVisible = false;
            ConfirmPasswordError.IsVisible = false;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(Nombre.Text))
            {
                NombreError.Text = "Nombre requerido";
                NombreError.IsVisible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Apellido.Text))
            {
                ApellidoError.Text = "Apellido requerido";
                ApellidoError.IsVisible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email.Text) || !Validators.IsValidEmail(Email.Text))
            {
                EmailError.Text = "Email inválido";
                EmailError.IsVisible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Comfirmacion_Email.Text) || Comfirmacion_Email.Text != Email.Text)
            {
                ConfirmEmailError.Text = "Los emails no coinciden";
                ConfirmEmailError.IsVisible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Password.Text) || !Validators.IsValidPassword(Password.Text))
            {
                PasswordError.Text = "Mínimo 8 caracteres, mayúscula, número y símbolo";
                PasswordError.IsVisible = true;
                isValid = false;
            }

            if (Confirmar_Password.Text != Password.Text)
            {
                ConfirmPasswordError.Text = "Las contraseñas no coinciden";
                ConfirmPasswordError.IsVisible = true;
                isValid = false;
            }

            if (!AcceptTerms.IsChecked)
            {
                DisplayAlert("Atención", "Debes aceptar los términos y condiciones", "OK");
                isValid = false;
            }

            return isValid;
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            var password = e.NewTextValue;
            var strength = Validators.GetPasswordStrengthMessage(password);
            
            PasswordStrength.Text = strength;
            PasswordStrength.TextColor = strength switch
            {
                "Fuerte" => Colors.Green,
                "Media" => Colors.Orange,
                _ => Colors.Red
            };
        }
    }
}
