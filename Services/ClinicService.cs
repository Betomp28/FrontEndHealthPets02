using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Service for veterinary clinic operations
    /// </summary>
    public class ClinicService : BaseApiService, IClinicService
    {
        public ClinicService() : base()
        {
        }

        /// <summary>
        /// Get all clinics
        /// </summary>
        public async Task<List<Clinic>> GetClinicsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CLINICS SERVICE: Calling API ===");
                var response = await GetAsync<ApiResponseWrapper<List<Clinic>>>("Clinics");
                System.Diagnostics.Debug.WriteLine($"=== CLINICS SERVICE: Response Success={response?.Success}, Count={response?.Data?.Count ?? 0} ===");
                
                if (response?.Data != null)
                {
                    foreach (var clinic in response.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Clinic: Id={clinic.Id}, Name={clinic.Name}, Vet={clinic.VeterinarianName}");
                    }
                }
                
                return response?.Data ?? new List<Clinic>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== CLINICS SERVICE ERROR: {ex.Message} ===");
                System.Diagnostics.Debug.WriteLine($"=== Stack: {ex.StackTrace} ===");
                return new List<Clinic>();
            }
        }

        /// <summary>
        /// Get a specific clinic by ID
        /// </summary>
        public async Task<Clinic?> GetClinicByIdAsync(int clinicId)
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<Clinic>>($"Clinics/{clinicId}");
                return response?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting clinic: {ex.Message}");
                return null;
            }
        }
    }
}
