using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Login with email and password
        /// </summary>
        Task<(bool success, User? user, string error)> LoginAsync(string email, string password);

        /// <summary>
        /// Register new user
        /// </summary>
        Task<(bool success, string error)> RegisterAsync(string nombre, string apellidos, string email, string password);

        /// <summary>
        /// Logout current user
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Refresh authentication token
        /// </summary>
        Task<bool> RefreshTokenAsync();

        /// <summary>
        /// Request password reset
        /// </summary>
        Task<(bool success, string error)> RequestPasswordResetAsync(string email);

        /// <summary>
        /// Reset password with code
        /// </summary>
        Task<(bool success, string error)> ResetPasswordAsync(string email, string code, string newPassword);

        /// <summary>
        /// Verify email with code
        /// </summary>
        Task<(bool success, string error)> VerifyEmailAsync(string email, string code);

        /// <summary>
        /// Resend verification code to email
        /// </summary>
        Task<(bool success, string error)> ResendVerificationCodeAsync(string email);

        /// <summary>
        /// Get current user
        /// </summary>
        Task<User?> GetCurrentUserAsync();

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }
    }
}
