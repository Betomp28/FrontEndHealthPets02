using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Authentication service for veterinarians
    /// </summary>
    public class VetAuthenticationService : BaseApiService, IVetAuthenticationService
    {
        public bool IsAuthenticated => Settings.IsVetAuthenticated;
        public int VetId => Settings.VetId;
        public string VetName => Settings.VetName;

        public async Task<(bool success, VetProfile? vet, string error)> LoginAsync(string email, string password)
        {
            try
            {
                var request = new
                {
                    Email = email,
                    Password = password
                };

                var response = await PostAsync<object, VetLoginResponse>("vet-auth/login", request);

                if (response == null || !response.Success)
                {
                    return (false, null, response?.Message ?? "Credenciales inválidas o error de conexión");
                }

                // Save vet auth data
                Settings.VetAuthToken = response.Token ?? "";
                Settings.VetId = response.VetId ?? 0;
                Settings.VetEmail = email;
                Settings.VetName = response.Name ?? "";
                Settings.VetSpecialty = response.Specialty ?? "";

                SetVetAuthenticationHeader();

                var vet = new VetProfile
                {
                    Id = response.VetId ?? 0,
                    Name = response.Name ?? "",
                    Email = email,
                    Specialty = response.Specialty,
                    Token = Settings.VetAuthToken
                };

                return (true, vet, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VetLogin] Error: {ex.Message}");
                return (false, null, Constants.NetworkErrorMessage);
            }
        }

        public Task LogoutAsync()
        {
            Settings.ClearVetAuth();
            return Task.CompletedTask;
        }

        public async Task<VetProfile?> GetProfileAsync()
        {
            if (!IsAuthenticated)
                return null;

            try
            {
                SetVetAuthenticationHeader();
                var response = await GetAsync<VetProfileResponse>("vet-auth/me");

                if (response == null)
                    return null;

                return new VetProfile
                {
                    Id = response.Id,
                    Name = response.Name,
                    Email = response.Email,
                    Phone = response.Phone,
                    Specialty = response.Specialty,
                    Description = response.Description,
                    ProfileImage = response.ProfileImage,
                    ConsultationPrice = response.ConsultationPrice,
                    ClinicName = response.ClinicName,
                    TodayAppointments = response.TodayAppointments,
                    PendingAppointments = response.PendingAppointments
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetVetProfile] Error: {ex.Message}");
                return null;
            }
        }

        private void SetVetAuthenticationHeader()
        {
            if (!string.IsNullOrEmpty(Settings.VetAuthToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Settings.VetAuthToken);
            }
        }

        // Response DTOs
        private class VetLoginResponse
        {
            public bool Success { get; set; }
            public string? Token { get; set; }
            public int? VetId { get; set; }
            public string? Name { get; set; }
            public string? Specialty { get; set; }
            public string? Message { get; set; }
        }

        private class VetProfileResponse
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Specialty { get; set; }
            public string? Description { get; set; }
            public string? ProfileImage { get; set; }
            public decimal ConsultationPrice { get; set; }
            public string? ClinicName { get; set; }
            public int TodayAppointments { get; set; }
            public int PendingAppointments { get; set; }
        }
    }
}
