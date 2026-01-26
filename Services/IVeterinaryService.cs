using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interface for veterinary clinic services
    /// </summary>
    public interface IVeterinaryService
    {
        /// <summary>
        /// Get nearby veterinary clinics
        /// </summary>
        Task<List<Veterinary>> GetNearbyVeterinariesAsync(double latitude, double longitude, double radiusKm = 10);

        /// <summary>
        /// Get veterinary details by ID
        /// </summary>
        Task<Veterinary?> GetVeterinaryByIdAsync(long id);

        /// <summary>
        /// Search veterinaries by name or specialty
        /// </summary>
        Task<List<Veterinary>> SearchVeterinariesAsync(string query);

        /// <summary>
        /// Get featured/promoted veterinaries
        /// </summary>
        Task<List<Veterinary>> GetFeaturedVeterinariesAsync();
    }
}
