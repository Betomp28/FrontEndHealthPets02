using System.Text.Json;
using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Service for managing scheduled reminders (recordatorios)
    /// </summary>
    public class RecordatorioService : BaseApiService, IRecordatorioService
    {
        public RecordatorioService() : base()
        {
        }

        /// <summary>
        /// Get all reminders for the current user
        /// </summary>
        public async Task<List<Recordatorio>> GetRecordatoriosAsync()
        {
            try
            {
                var response = await GetAsync<ApiResponseWrapper<List<Recordatorio>>>("api/Notificaciones/recordatorios");
                return response?.Data ?? new List<Recordatorio>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting recordatorios: {ex.Message}");
                return new List<Recordatorio>();
            }
        }

        /// <summary>
        /// Create a new reminder
        /// </summary>
        public async Task<(bool success, Recordatorio? recordatorio, string error)> CreateRecordatorioAsync(CreateRecordatorioRequest request)
        {
            try
            {
                var response = await PostAsync<CreateRecordatorioRequest, ApiResponseWrapper<Recordatorio>>(
                    "api/Notificaciones/recordatorios", request);

                if (response?.Success == true)
                {
                    return (true, response.Data, "");
                }

                return (false, null, response?.Message ?? "Error al crear recordatorio");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating recordatorio: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Cancel an existing reminder
        /// </summary>
        public async Task<bool> CancelRecordatorioAsync(int recordatorioId)
        {
            try
            {
                // For POST without body, use empty object
                var response = await PostAsync<object, ApiResponseWrapper<object>>(
                    $"api/Notificaciones/recordatorios/{recordatorioId}/cancel", new { });

                return response?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling recordatorio: {ex.Message}");
                return false;
            }
        }
    }
    // Note: ApiResponseWrapper<T> is defined in PetService.cs
}
