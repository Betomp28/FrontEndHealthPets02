using FrontEndHealthPets.Models;
using FrontEndHealthPets.Helpers;
using System.Net.Http.Json;

namespace FrontEndHealthPets.Services
{
    public class PetService : BaseApiService, IPetService
    {
        public PetService() : base()
        {
        }

        private string GetFullImageUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.StartsWith("http")) return relativePath;

            var baseUrl = Helpers.Constants.ApiBaseUrl.Replace("api/", "", StringComparison.OrdinalIgnoreCase);
            return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        public async Task<List<Pet>> GetPetsAsync()
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<List<PetDto>>>("Pets");
                
                if (response?.Data == null) return new List<Pet>();

                return response.Data.Select(m => new Pet
                {
                    Id = m.Id,
                    Name = m.Name,
                    Species = m.Species,
                    Breed = m.Breed,
                    PhotoUrl = GetFullImageUrl(m.ProfileImage),
                    BirthDate = m.BirthDate.GetValueOrDefault(),
                    Weight = m.Weight,
                    Color = m.Color ?? string.Empty,
                    Gender = m.Gender ?? string.Empty,
                    MicrochipNumber = m.MicrochipNumber ?? string.Empty,
                    Notes = m.Notes ?? string.Empty,
                    PhotoOffsetY = m.PhotoOffsetY
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pets: {ex.Message}");
                return new List<Pet>();
            }
        }

        public async Task<Pet?> GetPetByIdAsync(long petId)
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<PetDto>>($"Pets/{petId}");
                if (response?.Data == null) return null;

                return new Pet
                {
                    Id = response.Data.Id,
                    Name = response.Data.Name,
                    Species = response.Data.Species,
                    Breed = response.Data.Breed,
                    PhotoUrl = GetFullImageUrl(response.Data.ProfileImage),
                    BirthDate = response.Data.BirthDate.GetValueOrDefault(),
                    Weight = response.Data.Weight,
                    Color = response.Data.Color ?? string.Empty,
                    Gender = response.Data.Gender ?? string.Empty,
                    MicrochipNumber = response.Data.MicrochipNumber ?? string.Empty,
                    Notes = response.Data.Notes ?? string.Empty,
                    PhotoOffsetY = response.Data.PhotoOffsetY
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool success, Pet? pet, string error)> AddPetAsync(Pet pet)
        {
            try
            {
                var request = new
                {
                    Name = pet.Name,
                    Species = pet.Species,
                    Breed = pet.Breed,
                    BirthDate = pet.BirthDate,
                    Weight = pet.Weight,
                    Color = pet.Color,
                    Gender = pet.Gender,
                    MicrochipNumber = pet.MicrochipNumber,
                    Notes = pet.Notes
                };

                var response = await PostAsync<object, ApiResponseWrapper<PetDto>>("Pets", request);
                
                if (response == null || !response.Success)
                    return (false, null, response?.Message ?? "Error al agregar mascota");

                pet.Id = response.Data?.Id ?? 0;
                return (true, pet, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding pet: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool success, string error)> UpdatePetAsync(Pet pet)
        {
            try
            {
                var request = new
                {
                    Name = pet.Name,
                    Species = pet.Species,
                    Breed = pet.Breed,
                    BirthDate = pet.BirthDate,
                    Weight = pet.Weight,
                    Color = pet.Color,
                    Gender = pet.Gender,
                    MicrochipNumber = pet.MicrochipNumber,
                    Notes = pet.Notes
                };

                var response = await PutAsync<object, ApiResponseWrapper<PetDto>>($"Pets/{pet.Id}", request);
                
                if (response == null || !response.Success)
                    return (false, response?.Message ?? "Error al actualizar mascota");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating pet: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string error)> DeletePetAsync(long petId)
        {
            try
            {
                var success = await DeleteAsync($"Pets/{petId}");
                
                if (!success)
                    return (false, "Error al eliminar mascota");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting pet: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string? photoUrl, string error)> UploadPhotoAsync(long petId, Stream photoStream, string fileName)
        {
            try
            {
                var additionalData = new Dictionary<string, string>
                {
                    { "description", "Foto de galería" }
                };

                var result = await UploadFileAsync<PetPhoto>($"PetPhotos/{petId}", photoStream, fileName, additionalData);
                
                if (result != null)
                {
                    return (true, GetFullImageUrl(result.PhotoUrl), string.Empty);
                }
                
                return (false, null, "Error al subir foto");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading photo: {ex.Message}");
                return (false, null, ex.Message);
            }
        }
        
        // Helper method for profile image specifically
        public async Task<(bool success, string? photoUrl, string error)> UploadProfileImageAsync(long petId, Stream photoStream, string fileName)
        {
            try
            {
                // Use PetDto because backend returns the whole Pet object, 
                // and compute ProfileImage in frontend is read-only.
                var result = await UploadFileAsync<PetDto>($"PetPhotos/{petId}/profile-image", photoStream, fileName);
                
                if (result != null && !string.IsNullOrEmpty(result.ProfileImage))
                {
                    return (true, GetFullImageUrl(result.ProfileImage), string.Empty);
                }
                
                return (false, null, "Error al actualizar imagen de perfil");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading profile image: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool success, string error)> UpdatePhotoOffsetAsync(long petId, double offset)
        {
            try
            {
                // PutAsync expects a response body to deserialize, but the backend returns Ok() (empty).
                // We'll use HttpClient directly here to handle the empty response.
                SetAuthenticationHeader();
                
                var response = await _httpClient.PutAsJsonAsync($"PetPhotos/{petId}/photo-offset", offset, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"UpdatePhotoOffsetAsync failed: {response.StatusCode} - {errorContent}");
                
                return (false, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating photo offset: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<int> GetPetCountAsync()
        {
            var pets = await GetPetsAsync();
            return pets.Count;
        }

        public async Task<bool> CanAddMorePetsAsync()
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Vaccine methods
        public async Task<List<VaccineRecord>> GetPetVaccinesAsync(long petId)
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<List<VaccineRecordResponse>>>($"pets/{petId}/vaccines");
                if (response?.Data == null) return new List<VaccineRecord>();

                return response.Data.Select(v => new VaccineRecord
                {
                    Id = v.Id,
                    VaccineName = v.VaccineName,
                    Dose = v.Dose,
                    DateAdministered = v.DateAdministered ?? DateTime.MinValue,
                    NextDueDate = v.NextDueDate,
                    Notes = v.Notes
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting vaccines: {ex.Message}");
                return new List<VaccineRecord>();
            }
        }

        public async Task<VaccineRecord?> AddVaccineAsync(long petId, VaccineRecord vaccine)
        {
            try
            {
                var request = new
                {
                    VaccineName = vaccine.VaccineName,
                    Dose = vaccine.Dose,
                    DateAdministered = vaccine.DateAdministered,
                    NextDueDate = vaccine.NextDueDate,
                    Notes = vaccine.Notes
                };

                var response = await PostAsync<object, ApiResponseWrapper<VaccineRecordResponse>>($"pets/{petId}/vaccines", request);
                if (response?.Data == null) return null;

                vaccine.Id = response.Data.Id;
                return vaccine;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding vaccine: {ex.Message}");
                return null;
            }
        }

        // Appointment methods
        public async Task<List<Appointment>> GetPetAppointmentsAsync(long petId)
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<List<AppointmentResponse>>>("Appointments");
                if (response?.Data == null) return new List<Appointment>();

                // Filter by petId if needed, though backend currently returns all for user
                return response.Data
                    .Where(a => a.PetId == petId)
                    .Select(a => new Appointment
                    {
                        Id = a.Id,
                        PetId = a.PetId,
                        ClinicId = a.ClinicId,
                        ClinicName = a.ClinicName,
                        Date = a.DateTime.Date,
                        Time = a.DateTime.TimeOfDay,
                        Reason = a.Reason ?? string.Empty,
                        Notes = a.Notes ?? string.Empty
                    }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting appointments: {ex.Message}");
                return new List<Appointment>();
            }
        }

        public async Task<Appointment?> AddAppointmentAsync(long petId, Appointment appointment)
        {
            try
            {
                var request = new
                {
                    PetId = petId,
                    ClinicId = appointment.ClinicId,
                    DateTime = appointment.DateTime,
                    Reason = appointment.Reason,
                    Notes = appointment.Notes
                };

                var response = await PostAsync<object, ApiResponseWrapper<AppointmentResponse>>("Appointments", request);
                if (response?.Data == null) return null;

                appointment.Id = response.Data.Id;
                return appointment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding appointment: {ex.Message}");
                return null;
            }
        }

        // Photo gallery methods
        public async Task<List<PetPhoto>> GetPetPhotosAsync(long petId)
        {
            try
            {
                var response = await GetAsync<List<PetPhoto>>($"PetPhotos/{petId}");
                var photos = response ?? new List<PetPhoto>();
                foreach (var photo in photos)
                {
                    photo.PhotoUrl = GetFullImageUrl(photo.PhotoUrl);
                }
                return photos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting photos: {ex.Message}");
                return new List<PetPhoto>();
            }
        }

        public async Task<bool> DeletePetPhotoAsync(long petId, long photoId)
        {
            try
            {
                var response = await DeleteAsync($"PetPhotos/{photoId}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting photo: {ex.Message}");
                return false;
            }
        }
    }

    // Helper classes matching new ASP.NET Core backend
    public class ApiResponseWrapper<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class PetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public string Breed { get; set; } = "";
        public DateTime? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
        public int? Age { get; set; }
        public double Weight { get; set; }
        public string? Color { get; set; }
        public string? Gender { get; set; }
        public string? MicrochipNumber { get; set; }
        public string? Notes { get; set; }
        public double PhotoOffsetY { get; set; }
    }

    // Legacy response class for backward compatibility
    public class PetResponse
    {
        public bool resultado { get; set; }
        public string? error { get; set; }
    }

    public class VaccineListResponse : PetResponse
    {
        public List<VaccineRecord> Lista_Vacunas { get; set; } = new();
    }

    public class AppointmentListResponse : PetResponse
    {
        public List<Appointment> Lista_Citas { get; set; } = new();
    }

    public class VaccineRecordResponse
    {
        public int Id { get; set; }
        public string VaccineName { get; set; } = "";
        public int Dose { get; set; }
        public DateTime? DateAdministered { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentResponse
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public int ClinicId { get; set; }
        public string PetName { get; set; } = "";
        public string ClinicName { get; set; } = "";
        public DateTime DateTime { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class PhotoUploadResponse
    {
        public string PhotoUrl { get; set; } = string.Empty;
        public long PhotoId { get; set; }
    }


}
