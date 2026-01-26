using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interface for clinic operations
    /// </summary>
    public interface IClinicService
    {
        /// <summary>
        /// Get all clinics
        /// </summary>
        Task<List<Clinic>> GetClinicsAsync();

        /// <summary>
        /// Get a specific clinic by ID
        /// </summary>
        Task<Clinic?> GetClinicByIdAsync(int clinicId);
    }
}
