using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interface for managing scheduled reminders (recordatorios)
    /// </summary>
    public interface IRecordatorioService
    {
        /// <summary>
        /// Get all reminders for the current user
        /// </summary>
        Task<List<Recordatorio>> GetRecordatoriosAsync();

        /// <summary>
        /// Create a new reminder
        /// </summary>
        Task<(bool success, Recordatorio? recordatorio, string error)> CreateRecordatorioAsync(CreateRecordatorioRequest request);

        /// <summary>
        /// Cancel an existing reminder
        /// </summary>
        Task<bool> CancelRecordatorioAsync(int recordatorioId);
    }

    /// <summary>
    /// Request model for creating a new reminder
    /// </summary>
    public class CreateRecordatorioRequest
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "GENERAL"; // VACUNA, CITA, MEDICAMENTO, GENERAL
        public DateTime ReminderDate { get; set; }
        public int? PetId { get; set; }
        public bool Recurring { get; set; }
        public string? RecurrencePattern { get; set; } // DAILY, WEEKLY, MONTHLY
    }
}
