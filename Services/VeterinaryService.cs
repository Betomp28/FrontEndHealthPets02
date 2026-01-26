using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Veterinary service implementation with sample data
    /// TODO: Connect to real backend API when available
    /// </summary>
    public class VeterinaryService : BaseApiService, IVeterinaryService
    {
        public VeterinaryService() : base()
        {
        }

        public async Task<List<Veterinary>> GetNearbyVeterinariesAsync(double latitude, double longitude, double radiusKm = 10)
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<List<ClinicDto>>>("Clinics");
                if (response?.Data == null) return new List<Veterinary>();

                // Simplified: for now just return all clinics from backend as "nearby"
                return response.Data.Select(c => new Veterinary
                {
                    Id = c.Id,
                    Nombre = c.Name,
                    Direccion = c.Address,
                    Telefono = c.Phone,
                    Rating = 4.5, // Default for now
                    ReviewCount = 0
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting clinics: {ex.Message}");
                return new List<Veterinary>();
            }
        }

        public async Task<Veterinary?> GetVeterinaryByIdAsync(long id)
        {
            var clinics = await GetNearbyVeterinariesAsync(0, 0);
            return clinics.FirstOrDefault(v => v.Id == id);
        }

        public async Task<List<Veterinary>> SearchVeterinariesAsync(string query)
        {
            var clinics = await GetNearbyVeterinariesAsync(0, 0);
            if (string.IsNullOrWhiteSpace(query))
                return clinics;

            return clinics.Where(v => 
                v.Nombre.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.Direccion.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<Veterinary>> GetFeaturedVeterinariesAsync()
        {
            var clinics = await GetNearbyVeterinariesAsync(0, 0);
            return clinics.Take(3).ToList();
        }

        // Keep helper classes internal to service if not used elsewhere
        private class ClinicDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string? DoctorName { get; set; }
            public string Address { get; set; } = "";
            public string Phone { get; set; } = "";
        }
    }
}
