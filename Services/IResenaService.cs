using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interfaz para el servicio de reseñas/calificaciones
    /// </summary>
    public interface IResenaService
    {
        /// <summary>
        /// Obtiene las reseñas de un veterinario
        /// </summary>
        Task<List<Resena>> GetResenasByVeterinarioAsync(int vetId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtiene el resumen de calificaciones de un veterinario
        /// </summary>
        Task<VeterinarianRatingSummary?> GetVeterinarioRatingSummaryAsync(int vetId);

        /// <summary>
        /// Crea una nueva reseña
        /// </summary>
        Task<(bool success, Resena? resena, string error)> CreateResenaAsync(ResenaCreateRequest request);

        /// <summary>
        /// Obtiene una reseña específica
        /// </summary>
        Task<Resena?> GetResenaAsync(int id);

        /// <summary>
        /// Actualiza una reseña
        /// </summary>
        Task<(bool success, string error)> UpdateResenaAsync(int id, ResenaCreateRequest request);

        /// <summary>
        /// Elimina una reseña
        /// </summary>
        Task<bool> DeleteResenaAsync(int id);

        /// <summary>
        /// El veterinario responde a una reseña
        /// </summary>
        Task<(bool success, string error)> RespondToResenaAsync(int id, string response);

        /// <summary>
        /// Obtiene las reseñas del usuario actual
        /// </summary>
        Task<List<Resena>> GetMyReviewsAsync();

        /// <summary>
        /// Obtiene las reseñas del veterinario actual
        /// </summary>
        Task<List<Resena>> GetVetReviewsAsync();

        /// <summary>
        /// Verifica si el usuario puede dejar reseña para una cita
        /// </summary>
        Task<bool> CanReviewAppointmentAsync(int appointmentId);
    }
}
