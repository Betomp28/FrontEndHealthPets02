using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Pet management service interface
    /// </summary>
    public interface IPetService
    {
        /// <summary>
        /// Get all pets for current user
        /// </summary>
        Task<List<Pet>> GetPetsAsync();

        /// <summary>
        /// Get pet by ID
        /// </summary>
        Task<Pet?> GetPetByIdAsync(long petId);

        /// <summary>
        /// Add new pet
        /// </summary>
        Task<(bool success, Pet? pet, string error)> AddPetAsync(Pet pet);

        /// <summary>
        /// Update pet
        /// </summary>
        Task<(bool success, string error)> UpdatePetAsync(Pet pet);

        /// <summary>
        /// Delete pet
        /// </summary>
        Task<(bool success, string error)> DeletePetAsync(long petId);

        /// <summary>
        /// Upload pet photo
        /// </summary>
        Task<(bool success, string? photoUrl, string error)> UploadPhotoAsync(long petId, Stream photoStream, string fileName);

        /// <summary>
        /// Upload pet profile image
        /// </summary>
        Task<(bool success, string? photoUrl, string error)> UploadProfileImageAsync(long petId, Stream photoStream, string fileName);

        /// <summary>
        /// Get pet count for current user
        /// </summary>
        Task<int> GetPetCountAsync();

        /// <summary>
        /// Check if user can add more pets (based on premium status)
        /// </summary>
        Task<bool> CanAddMorePetsAsync();

        /// <summary>
        /// Get vaccines for a pet
        /// </summary>
        Task<List<VaccineRecord>> GetPetVaccinesAsync(long petId);

        /// <summary>
        /// Add vaccine record
        /// </summary>
        Task<VaccineRecord?> AddVaccineAsync(long petId, VaccineRecord vaccine);

        /// <summary>
        /// Get appointments for a pet
        /// </summary>
        Task<List<Appointment>> GetPetAppointmentsAsync(long petId);

        /// <summary>
        /// Add appointment
        /// </summary>
        Task<Appointment?> AddAppointmentAsync(long petId, Appointment appointment);

        /// <summary>
        /// Get pet photos
        /// </summary>
        Task<List<PetPhoto>> GetPetPhotosAsync(long petId);

        /// <summary>
        /// Delete pet photo
        /// </summary>
        Task<bool> DeletePetPhotoAsync(long petId, long photoId);

        /// <summary>
        /// Update the vertical offset of the pet photo
        /// </summary>
        /// <summary>
        /// Update the vertical offset of the pet photo
        /// </summary>
        Task<(bool success, string error)> UpdatePhotoOffsetAsync(long petId, double offset);
    }
}
