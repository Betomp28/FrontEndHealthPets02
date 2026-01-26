using FrontEndHealthPets.Models;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Servicio para gestión de reseñas/calificaciones
    /// </summary>
    public class ResenaService : BaseApiService, IResenaService
    {
        public ResenaService() : base()
        {
        }

        /// <summary>
        /// Obtiene las reseñas de un veterinario
        /// </summary>
        public async Task<List<Resena>> GetResenasByVeterinarioAsync(int vetId, int page = 1, int pageSize = 10)
        {
            try
            {
                var endpoint = $"Resenas/veterinario/{vetId}?page={page}&pageSize={pageSize}";
                var response = await GetAsync<List<ResenaDto>>(endpoint);
                return response?.Select(MapResenaFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting resenas: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Obtiene el resumen de calificaciones de un veterinario
        /// </summary>
        public async Task<VeterinarianRatingSummary?> GetVeterinarioRatingSummaryAsync(int vetId)
        {
            try
            {
                var response = await GetAsync<VeterinarianRatingSummaryDto>($"Resenas/veterinario/{vetId}/summary");
                if (response == null) return null;

                return new VeterinarianRatingSummary
                {
                    VeterinarianId = response.VeterinarianId,
                    VeterinarianName = response.VeterinarianName,
                    AverageRating = response.AverageRating,
                    TotalReviews = response.TotalReviews,
                    FiveStarCount = response.FiveStarCount,
                    FourStarCount = response.FourStarCount,
                    ThreeStarCount = response.ThreeStarCount,
                    TwoStarCount = response.TwoStarCount,
                    OneStarCount = response.OneStarCount,
                    AveragePunctuality = response.AveragePunctuality,
                    AverageAttention = response.AverageAttention,
                    AverageExplanation = response.AverageExplanation,
                    AverageFacilities = response.AverageFacilities
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting rating summary: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea una nueva reseña
        /// </summary>
        public async Task<(bool success, Resena? resena, string error)> CreateResenaAsync(ResenaCreateRequest request)
        {
            try
            {
                var response = await PostAsync<ResenaCreateRequest, ResenaDto>("Resenas", request);
                if (response == null)
                    return (false, null, "Error al crear reseña");

                return (true, MapResenaFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating resena: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una reseña específica
        /// </summary>
        public async Task<Resena?> GetResenaAsync(int id)
        {
            try
            {
                var response = await GetAsync<ResenaDto>($"Resenas/{id}");
                return response != null ? MapResenaFromDto(response) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting resena: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Actualiza una reseña
        /// </summary>
        public async Task<(bool success, string error)> UpdateResenaAsync(int id, ResenaCreateRequest request)
        {
            try
            {
                var response = await PutAsync<ResenaCreateRequest, ResenaDto>($"Resenas/{id}", request);
                if (response == null)
                    return (false, "Error al actualizar reseña");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating resena: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una reseña
        /// </summary>
        public async Task<bool> DeleteResenaAsync(int id)
        {
            try
            {
                return await DeleteAsync($"Resenas/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting resena: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// El veterinario responde a una reseña
        /// </summary>
        public async Task<(bool success, string error)> RespondToResenaAsync(int id, string response)
        {
            try
            {
                var request = new { Response = response };
                var result = await PostAsync<object, ResenaDto>($"Resenas/{id}/response", request);
                if (result == null)
                    return (false, "Error al responder a la reseña");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error responding to resena: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene las reseñas del usuario actual
        /// </summary>
        public async Task<List<Resena>> GetMyReviewsAsync()
        {
            try
            {
                var response = await GetAsync<List<ResenaDto>>("Resenas/my-reviews");
                return response?.Select(MapResenaFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting my reviews: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Obtiene las reseñas del veterinario actual
        /// </summary>
        public async Task<List<Resena>> GetVetReviewsAsync()
        {
            try
            {
                var response = await GetAsync<List<ResenaDto>>("Resenas/vet/my-reviews");
                return response?.Select(MapResenaFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting vet reviews: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Verifica si el usuario puede dejar reseña para una cita
        /// </summary>
        public async Task<bool> CanReviewAppointmentAsync(int appointmentId)
        {
            try
            {
                var response = await GetAsync<bool>($"Resenas/can-review/{appointmentId}");
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking can review: {ex.Message}");
                return false;
            }
        }

        #region Mappers

        private Resena MapResenaFromDto(ResenaDto dto)
        {
            return new Resena
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName,
                VeterinarianId = dto.VeterinarianId,
                VeterinarianName = dto.VeterinarianName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                PunctualityRating = dto.PunctualityRating,
                AttentionRating = dto.AttentionRating,
                ExplanationRating = dto.ExplanationRating,
                FacilitiesRating = dto.FacilitiesRating,
                IsVerified = dto.IsVerified,
                VetResponse = dto.VetResponse,
                VetResponseDate = dto.VetResponseDate,
                ReviewDate = dto.ReviewDate
            };
        }

        #endregion
    }

    #region DTOs

    public class ResenaDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = "";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int? PunctualityRating { get; set; }
        public int? AttentionRating { get; set; }
        public int? ExplanationRating { get; set; }
        public int? FacilitiesRating { get; set; }
        public bool IsVerified { get; set; }
        public string? VetResponse { get; set; }
        public DateTime? VetResponseDate { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class VeterinarianRatingSummaryDto
    {
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = "";
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public decimal? AveragePunctuality { get; set; }
        public decimal? AverageAttention { get; set; }
        public decimal? AverageExplanation { get; set; }
        public decimal? AverageFacilities { get; set; }
    }

    public class ResenaCreateRequest
    {
        public int VeterinarianId { get; set; }
        public int? VetAppointmentId { get; set; }
        public int? ConsultaId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int? PunctualityRating { get; set; }
        public int? AttentionRating { get; set; }
        public int? ExplanationRating { get; set; }
        public int? FacilitiesRating { get; set; }

        // Alias properties for backward compatibility (Spanish names)
        public int VeterinarioId
        {
            get => VeterinarianId;
            set => VeterinarianId = value;
        }

        public int Calificacion
        {
            get => Rating;
            set => Rating = value;
        }

        public string? Comentario
        {
            get => Comment;
            set => Comment = value;
        }
    }

    #endregion
}
