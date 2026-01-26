using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Authentication service interface for veterinarians
    /// </summary>
    public interface IVetAuthenticationService
    {
        /// <summary>
        /// Login with email and password
        /// </summary>
        Task<(bool success, VetProfile? vet, string error)> LoginAsync(string email, string password);

        /// <summary>
        /// Logout current veterinarian
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get current veterinarian profile
        /// </summary>
        Task<VetProfile?> GetProfileAsync();

        /// <summary>
        /// Check if veterinarian is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Get stored vet ID
        /// </summary>
        int VetId { get; }

        /// <summary>
        /// Get stored vet name
        /// </summary>
        string VetName { get; }
    }
}
