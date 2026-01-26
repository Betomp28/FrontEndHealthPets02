using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthenticationService : BaseApiService, IAuthenticationService
    {
        public bool IsAuthenticated => Settings.IsAuthenticated;

        public async Task<(bool success, User? user, string error)> LoginAsync(string email, string password)
        {
            try
            {
                var request = new
                {
                    Email = email,
                    Password = password
                };

                var response = await PostAsync<object, LoginResponse>("Auth/login", request);

                if (response == null || !response.Success)
                {
                    return (false, null, response?.Message ?? "Credenciales inválidas o error de conexión");
                }

                // Save auth data
                Settings.AuthToken = response.Token ?? "";
                Settings.UserId = response.UserId ?? 0;
                Settings.UserEmail = email;
                Settings.UserName = response.Name ?? "";

                SetAuthenticationHeader();

                var user = new User
                {
                    Id = response.UserId ?? 0,
                    Nombre = response.Name ?? "",
                    Apellidos = "",
                    Email = email,
                    Token = Settings.AuthToken
                };

                return (true, user, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return (false, null, Constants.NetworkErrorMessage);
            }
        }

        public async Task<(bool success, string error)> RegisterAsync(string nombre, string apellidos, string email, string password)
        {
            try
            {
                var request = new
                {
                    Name = nombre,
                    LastName = apellidos,
                    Email = email,
                    Password = password,
                    ConfirmPassword = password
                };

                var response = await PostAsync<object, RegisterResponse>("Auth/register", request);

                if (response == null || !response.Success)
                {
                    return (false, response?.Message ?? "Error al registrar usuario");
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Register] Error: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Register] Inner: {ex.InnerException.Message}");
                }
                return (false, Constants.NetworkErrorMessage);
            }
        }

        public Task LogoutAsync()
        {
            Settings.ClearAuth();
            return Task.CompletedTask;
        }

        public Task<bool> RefreshTokenAsync()
        {
            return Task.FromResult(false);
        }

        public async Task<(bool success, string error)> RequestPasswordResetAsync(string email)
        {
            try
            {
                var request = new { Email = email };
                var response = await PostAsync<object, PasswordResetResponse>("Auth/forgot-password", request);

                if (response == null || !response.Success)
                {
                    return (false, response?.Message ?? "Error al solicitar recuperación de contraseña");
                }

                return (true, response.Message ?? "Código enviado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestPasswordReset] Error: {ex.Message}");
                return (false, Constants.NetworkErrorMessage);
            }
        }

        public async Task<(bool success, string error)> ResetPasswordAsync(string email, string code, string newPassword)
        {
            try
            {
                var request = new { Email = email, Code = code, NewPassword = newPassword };
                var response = await PostAsync<object, PasswordResetResponse>("Auth/reset-password", request);

                if (response == null || !response.Success)
                {
                    return (false, response?.Message ?? "Error al restablecer contraseña");
                }

                return (true, response.Message ?? "Contraseña actualizada");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ResetPassword] Error: {ex.Message}");
                return (false, Constants.NetworkErrorMessage);
            }
        }

        public async Task<(bool success, string error)> VerifyEmailAsync(string email, string code)
        {
            try
            {
                var request = new { Email = email, Code = code };
                var response = await PostAsync<object, VerifyEmailResponse>("Auth/verify-email", request);

                if (response == null || !response.Success)
                {
                    return (false, response?.Message ?? "Error al verificar el correo");
                }

                // If verification returns a token, save it for auto-login
                if (!string.IsNullOrEmpty(response.Token))
                {
                    Settings.AuthToken = response.Token;
                    Settings.UserId = response.UserId ?? 0;
                    Settings.UserName = response.Name ?? "";
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VerifyEmail] Error: {ex.Message}");
                return (false, Constants.NetworkErrorMessage);
            }
        }

        public async Task<(bool success, string error)> ResendVerificationCodeAsync(string email)
        {
            try
            {
                var request = new { Email = email };
                var response = await PostAsync<object, VerifyEmailResponse>("Auth/resend-code", request);

                if (response == null || !response.Success)
                {
                    return (false, response?.Message ?? "Error al reenviar el código");
                }

                return (true, response.Message ?? "Código reenviado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ResendCode] Error: {ex.Message}");
                return (false, Constants.NetworkErrorMessage);
            }
        }

        public Task<User?> GetCurrentUserAsync()
        {
            if (!IsAuthenticated)
                return Task.FromResult<User?>(null);

            var user = new User
            {
                Id = Settings.UserId,
                Email = Settings.UserEmail,
                Nombre = Settings.UserName.Split(' ').FirstOrDefault() ?? "",
                Apellidos = string.Join(" ", Settings.UserName.Split(' ').Skip(1))
            };

            return Task.FromResult<User?>(user);
        }

        // Response DTOs matching new ASP.NET Core backend
        private class LoginResponse
        {
            public bool Success { get; set; }
            public string? Token { get; set; }
            public int? UserId { get; set; }
            public string? Name { get; set; }
            public string? Message { get; set; }
        }

        private class RegisterResponse
        {
            public bool Success { get; set; }
            public string? Token { get; set; }
            public int? UserId { get; set; }
            public string? Name { get; set; }
            public string? Message { get; set; }
        }

        // Same structure as RegisterResponse
        private class VerifyEmailResponse
        {
            public bool Success { get; set; }
            public string? Token { get; set; }
            public int? UserId { get; set; }
            public string? Name { get; set; }
            public string? Message { get; set; }
        }

        private class PasswordResetResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }
    }
}
