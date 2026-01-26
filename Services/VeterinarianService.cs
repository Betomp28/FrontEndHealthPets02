using FrontEndHealthPets.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontEndHealthPets.Services
{
    public class VeterinarianService : IVeterinarianService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "https://localhost:7198/api"; // Adjust port if needed, usually matches generic HttpClient setup
        // Note: In a real app, BaseUrl should come from configuration or a shared constant

        public VeterinarianService()
        {
            // We'll rely on the HttpClient being configured with the base address in MauiProgram, or use a specific one
            // Ideally receiving IHttpClientFactory or a pre-configured HttpClient
            
            // For simplicity in this existing structure, I'll assume we use a fresh client or inherit from a BaseService if one existed.
            // But looking at AuthenticationService, it seems to do its own thing. 
            // Let's implement fully but using the AuthenticationService's token if available.
            
            var handler = new HttpClientHandler();
            
            // Bypass SSL for local development
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            
            _httpClient = new HttpClient(handler);
            
            // Set base address - attempting to read from generic constants or hardcoding for now based on previous context
            // Backend seems to be running on http://localhost:5152 or https://localhost:7198. 
            // QA tests used 5152 (http). Let's try to be flexible but default to 5152 for the emulator (10.0.2.2) if on Android.
            
#if ANDROID
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:5152/api/");
#else
            _httpClient.BaseAddress = new Uri("http://localhost:5152/api/");
#endif
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<Veterinarian>> GetVeterinariansAsync(string? specialty = null)
        {
            try
            {
                var url = "Veterinarians";
                if (!string.IsNullOrEmpty(specialty))
                {
                    url += $"?specialty={Uri.EscapeDataString(specialty)}";
                }

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var vets = await response.Content.ReadFromJsonAsync<List<Veterinarian>>(_jsonOptions);
                    if (vets != null && vets.Count > 0)
                    {
                        return vets;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching vets: {ex.Message}");
            }

            // Retornar datos mock si la API no responde o está vacía
            return GetMockVeterinarians(specialty);
        }

        private List<Veterinarian> GetMockVeterinarians(string? specialty = null)
        {
            var mockVets = new List<Veterinarian>
            {
                new Veterinarian
                {
                    Id = 1,
                    Name = "Dr. Roberto Martínez",
                    Specialty = "Medicina General",
                    Description = "Veterinario con más de 15 años de experiencia en atención de mascotas. Especializado en perros y gatos. Atención personalizada y trato amable.",
                    Phone = "+52 555 123 4567",
                    Email = "dr.martinez@healthpets.com",
                    ConsultationPrice = 450.00m,
                    Rating = 4.8,
                    ReviewCount = 127,
                    ProfileImage = "https://randomuser.me/api/portraits/men/32.jpg",
                    ClinicName = "Clínica Veterinaria San Ángel",
                    ClinicAddress = "Av. Revolución 1425, San Ángel, CDMX",
                    IsActive = true,
                    YearsExperience = 15
                },
                new Veterinarian
                {
                    Id = 2,
                    Name = "Dra. Ana García López",
                    Specialty = "Dermatología",
                    Description = "Especialista en dermatología veterinaria. Tratamiento de alergias, problemas de piel y pelaje. Diplomada por la Universidad Nacional Autónoma de México.",
                    Phone = "+52 555 987 6543",
                    Email = "dra.garcia@healthpets.com",
                    ConsultationPrice = 550.00m,
                    Rating = 4.9,
                    ReviewCount = 89,
                    ProfileImage = "https://randomuser.me/api/portraits/women/44.jpg",
                    ClinicName = "VetCenter Polanco",
                    ClinicAddress = "Calle Horacio 1020, Polanco, CDMX",
                    IsActive = true,
                    YearsExperience = 10
                }
            };

            // Filtrar por especialidad si se especifica
            if (!string.IsNullOrEmpty(specialty))
            {
                return mockVets.Where(v => v.Specialty != null && v.Specialty.Contains(specialty, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return mockVets;
        }

        public async Task<Veterinarian?> GetVeterinarianByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Veterinarians/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Veterinarian>(_jsonOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching vet details: {ex.Message}");
            }
            return null;
        }

        public async Task<List<VetSchedule>> GetVetScheduleAsync(int vetId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"VetSchedule/{vetId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<VetSchedule>>(_jsonOptions) ?? new List<VetSchedule>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching schedule: {ex.Message}");
            }
            return new List<VetSchedule>();
        }

        public async Task<List<AvailableSlot>> GetAvailabilityAsync(int vetId, DateTime date)
        {
            try
            {
                var dateStr = date.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"Veterinarians/{vetId}/availability?date={dateStr}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<AvailableSlot>>(_jsonOptions) ?? new List<AvailableSlot>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching availability: {ex.Message}");
            }
            return new List<AvailableSlot>();
        }

        public async Task<(bool success, string message, VetAppointment? appointment)> BookAppointmentAsync(int vetId, DateTime dateTime, string reason, string? notes = null, int? petId = null)
        {
            try
            {
                await AddAuthHeaderAsync();

                var request = new
                {
                    VeterinarianId = vetId,
                    AppointmentDateTime = dateTime,
                    Reason = reason,
                    Notes = notes,
                    PetId = petId
                };

                var response = await _httpClient.PostAsJsonAsync("VetAppointments", request, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    var appointment = await response.Content.ReadFromJsonAsync<VetAppointment>(_jsonOptions);
                    return (true, "Cita agendada exitosamente", appointment);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, "Error al agendar cita: " + response.ReasonPhrase, null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}", null);
            }
        }

        public async Task<List<VetAppointment>> GetMyAppointmentsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("VetAppointments/my");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<VetAppointment>>(_jsonOptions) ?? new List<VetAppointment>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching appointments: {ex.Message}");
            }
            return new List<VetAppointment>();
        }

        public async Task<(bool success, string message)> CancelAppointmentAsync(int appointmentId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var request = new { Status = "Cancelled" };
                var response = await _httpClient.PutAsJsonAsync($"VetAppointments/{appointmentId}/status", request, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cita cancelada exitosamente");
                }
                return (false, "No se pudo cancelar la cita");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        
        public async Task<(bool success, string message)> ConfirmAppointmentAsync(int appointmentId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var request = new { Status = "Confirmed" };
                var response = await _httpClient.PutAsJsonAsync($"VetAppointments/{appointmentId}/status", request, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cita confirmada exitosamente");
                }
                return (false, "No se pudo confirmar la cita");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        // ===== VET PORTAL METHODS =====

        public async Task<(bool success, string? imageUrl, string? error)> UploadPhotoAsync(int vetId, Stream imageStream, string fileName)
        {
            try
            {
                await AddAuthHeaderAsync();

                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(imageStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync($"Veterinarians/{vetId}/photo", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                    var imageUrl = result.GetProperty("imageUrl").GetString();
                    return (true, imageUrl, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, null, error);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool success, string? error)> UpdateProfileAsync(int vetId, string name, string specialty, string description, string phone, decimal price)
        {
            try
            {
                await AddAuthHeaderAsync();

                var request = new
                {
                    Name = name,
                    Specialty = specialty,
                    Description = description,
                    Phone = phone,
                    ConsultationPrice = price
                };

                var response = await _httpClient.PutAsJsonAsync($"Veterinarians/{vetId}", request, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<VetSchedule>> GetSchedulesAsync(int vetId)
        {
            // Reuse existing method
            return await GetVetScheduleAsync(vetId);
        }

        public async Task<(bool success, string? error)> CreateScheduleAsync(int vetId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int slotDuration)
        {
            try
            {
                await AddAuthHeaderAsync();

                var request = new
                {
                    VeterinarianId = vetId,
                    DayOfWeek = dayOfWeek,
                    StartTime = startTime.ToString(@"hh\:mm\:ss"),
                    EndTime = endTime.ToString(@"hh\:mm\:ss"),
                    SlotDurationMinutes = slotDuration
                };

                var response = await _httpClient.PostAsJsonAsync("VetSchedule", request, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync($"VetSchedule/{scheduleId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<VetAppointment>> GetVetAppointmentsAsync(int vetId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync($"VetAppointments/vet/{vetId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<VetAppointment>>(_jsonOptions) ?? new List<VetAppointment>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching vet appointments: {ex.Message}");
            }
            return new List<VetAppointment>();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            try
            {
                await AddAuthHeaderAsync();
                var request = new { Status = status };
                var response = await _httpClient.PutAsJsonAsync($"VetAppointments/{appointmentId}/status", request, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating appointment status: {ex.Message}");
                return false;
            }
        }
    }
}
