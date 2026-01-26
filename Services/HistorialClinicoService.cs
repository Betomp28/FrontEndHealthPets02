using FrontEndHealthPets.Models;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Servicio para gestión de historial clínico y consultas
    /// </summary>
    public class HistorialClinicoService : BaseApiService, IHistorialClinicoService
    {
        public HistorialClinicoService() : base()
        {
        }

        private string GetFullImageUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.StartsWith("http")) return relativePath;

            var baseUrl = Constants.ApiBaseUrl.Replace("api/", "", StringComparison.OrdinalIgnoreCase);
            return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        /// <summary>
        /// Obtiene el historial clínico completo de una mascota
        /// </summary>
        public async Task<HistorialClinico?> GetHistorialClinicoAsync(long petId)
        {
            try
            {
                var response = await GetAsync<HistorialClinicoDto>($"Consultas/pet/{petId}/historial");
                if (response == null) return null;

                return new HistorialClinico
                {
                    Pet = MapPetFromDto(response.Pet),
                    OwnerName = response.OwnerName,
                    OwnerEmail = response.OwnerEmail,
                    Consultations = response.Consultations?.Select(MapConsultaFromDto).ToList() ?? new(),
                    Vaccines = response.Vaccines?.Select(v => new VaccineRecord
                    {
                        Id = v.Id,
                        VaccineName = v.VaccineName,
                        Dose = v.Dose,
                        DateAdministered = v.DateAdministered ?? DateTime.MinValue,
                        NextDueDate = v.NextDueDate,
                        Notes = v.Notes
                    }).ToList() ?? new(),
                    Medications = response.Medications?.Select(m => new MedicationRecord
                    {
                        Id = m.Id,
                        MedicationName = m.MedicationName,
                        Category = m.Category,
                        AdministrationMethod = m.AdministrationMethod,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        Notes = m.Notes
                    }).ToList() ?? new()
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting historial: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene las consultas de una mascota
        /// </summary>
        public async Task<List<Consulta>> GetConsultasByPetAsync(long petId)
        {
            try
            {
                var response = await GetAsync<List<ConsultaDto>>($"Consultas/pet/{petId}");
                return response?.Select(MapConsultaFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting consultas: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Obtiene una consulta específica
        /// </summary>
        public async Task<Consulta?> GetConsultaAsync(int id)
        {
            try
            {
                var response = await GetAsync<ConsultaDto>($"Consultas/{id}");
                return response != null ? MapConsultaFromDto(response) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting consulta: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea una nueva consulta (solo veterinarios)
        /// </summary>
        public async Task<(bool success, Consulta? consulta, string error)> CreateConsultaAsync(ConsultaCreateRequest request)
        {
            try
            {
                var response = await PostAsync<ConsultaCreateRequest, ConsultaDto>("Consultas", request);
                if (response == null)
                    return (false, null, "Error al crear consulta");

                return (true, MapConsultaFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating consulta: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una consulta
        /// </summary>
        public async Task<(bool success, string error)> UpdateConsultaAsync(int id, ConsultaCreateRequest request)
        {
            try
            {
                var response = await PutAsync<ConsultaCreateRequest, ConsultaDto>($"Consultas/{id}", request);
                if (response == null)
                    return (false, "Error al actualizar consulta");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating consulta: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los archivos de una consulta
        /// </summary>
        public async Task<List<ConsultaArchivo>> GetArchivosAsync(int consultaId)
        {
            try
            {
                var response = await GetAsync<List<ConsultaArchivoDto>>($"Consultas/{consultaId}/archivos");
                return response?.Select(a => new ConsultaArchivo
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileUrl = GetFullImageUrl(a.FileUrl),
                    Description = a.Description,
                    FileSizeKb = a.FileSizeKb,
                    UploadDate = a.UploadDate
                }).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting archivos: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Sube un archivo a una consulta
        /// </summary>
        public async Task<(bool success, ConsultaArchivo? archivo, string error)> UploadArchivoAsync(
            int consultaId, Stream fileStream, string fileName, string fileType, string? description)
        {
            try
            {
                var additionalData = new Dictionary<string, string>
                {
                    { "fileType", fileType }
                };

                if (!string.IsNullOrEmpty(description))
                    additionalData.Add("description", description);

                var response = await UploadFileAsync<ConsultaArchivoDto>(
                    $"Consultas/{consultaId}/archivos", fileStream, fileName, additionalData);

                if (response == null)
                    return (false, null, "Error al subir archivo");

                return (true, new ConsultaArchivo
                {
                    Id = response.Id,
                    FileName = response.FileName,
                    FileType = response.FileType,
                    FileUrl = GetFullImageUrl(response.FileUrl),
                    Description = response.Description,
                    FileSizeKb = response.FileSizeKb,
                    UploadDate = response.UploadDate
                }, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading archivo: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Elimina un archivo de una consulta
        /// </summary>
        public async Task<bool> DeleteArchivoAsync(int archivoId)
        {
            try
            {
                return await DeleteAsync($"Consultas/archivos/{archivoId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting archivo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene las consultas del veterinario actual
        /// </summary>
        public async Task<List<Consulta>> GetVetConsultasAsync(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var endpoint = "Consultas/vet/my-consultas";
                var queryParams = new List<string>();

                if (from.HasValue)
                    queryParams.Add($"from={from.Value:yyyy-MM-dd}");
                if (to.HasValue)
                    queryParams.Add($"to={to.Value:yyyy-MM-dd}");

                if (queryParams.Any())
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await GetAsync<List<ConsultaDto>>(endpoint);
                return response?.Select(MapConsultaFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting vet consultas: {ex.Message}");
                return new();
            }
        }

        #region Mappers

        private Pet MapPetFromDto(PetDto? dto)
        {
            if (dto == null) return new Pet();
            return new Pet
            {
                Id = dto.Id,
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                PhotoUrl = GetFullImageUrl(dto.ProfileImage),
                BirthDate = dto.BirthDate ?? DateTime.MinValue,
                Weight = dto.Weight,
                Color = dto.Color ?? string.Empty,
                Gender = dto.Gender ?? string.Empty,
                MicrochipNumber = dto.MicrochipNumber ?? string.Empty,
                Notes = dto.Notes ?? string.Empty,
                PhotoOffsetY = dto.PhotoOffsetY
            };
        }

        private Consulta MapConsultaFromDto(ConsultaDto dto)
        {
            return new Consulta
            {
                Id = dto.Id,
                PetId = dto.PetId,
                PetName = dto.PetName,
                PetSpecies = dto.PetSpecies,
                VeterinarianId = dto.VeterinarianId,
                VeterinarianName = dto.VeterinarianName,
                ClinicName = dto.ClinicName,
                ReasonForVisit = dto.ReasonForVisit,
                Symptoms = dto.Symptoms,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                Observations = dto.Observations,
                Recommendations = dto.Recommendations,
                CurrentWeight = dto.CurrentWeight,
                Temperature = dto.Temperature,
                HeartRate = dto.HeartRate,
                RespiratoryRate = dto.RespiratoryRate,
                ConsultationType = dto.ConsultationType,
                IsEmergency = dto.IsEmergency,
                ConsultationDate = dto.ConsultationDate,
                NextCheckupDate = dto.NextCheckupDate,
                ConsultationCost = dto.ConsultationCost,
                IsPaid = dto.IsPaid,
                Attachments = dto.Attachments?.Select(a => new ConsultaArchivo
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileUrl = GetFullImageUrl(a.FileUrl),
                    Description = a.Description,
                    FileSizeKb = a.FileSizeKb,
                    UploadDate = a.UploadDate
                }).ToList() ?? new()
            };
        }

        #endregion
    }

    #region DTOs

    public class ConsultaDto
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string PetName { get; set; } = "";
        public string PetSpecies { get; set; } = "";
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = "";
        public string? ClinicName { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Observations { get; set; }
        public string? Recommendations { get; set; }
        public decimal? CurrentWeight { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public string ConsultationType { get; set; } = "GENERAL";
        public bool IsEmergency { get; set; }
        public DateTime ConsultationDate { get; set; }
        public DateTime? NextCheckupDate { get; set; }
        public decimal? ConsultationCost { get; set; }
        public bool IsPaid { get; set; }
        public List<ConsultaArchivoDto>? Attachments { get; set; }
    }

    public class ConsultaArchivoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
        public string FileUrl { get; set; } = "";
        public string? Description { get; set; }
        public int? FileSizeKb { get; set; }
        public DateTime UploadDate { get; set; }
    }

    public class HistorialClinicoDto
    {
        public PetDto? Pet { get; set; }
        public string OwnerName { get; set; } = "";
        public string OwnerEmail { get; set; } = "";
        public List<ConsultaDto>? Consultations { get; set; }
        public List<VaccineRecordResponse>? Vaccines { get; set; }
        public List<MedicationRecordDto>? Medications { get; set; }
    }

    public class MedicationRecordDto
    {
        public int Id { get; set; }
        public string MedicationName { get; set; } = "";
        public string? Category { get; set; }
        public string? AdministrationMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
    }

    public class ConsultaCreateRequest
    {
        public int? VetAppointmentId { get; set; }
        public int PetId { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Observations { get; set; }
        public string? Recommendations { get; set; }
        public decimal? CurrentWeight { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public string ConsultationType { get; set; } = "GENERAL";
        public bool IsEmergency { get; set; }
        public DateTime? NextCheckupDate { get; set; }
        public decimal? ConsultationCost { get; set; }
    }

    #endregion
}
