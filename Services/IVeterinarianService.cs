using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    public interface IVeterinarianService
    {
        Task<List<Veterinarian>> GetVeterinariansAsync(string? specialty = null);
        Task<Veterinarian?> GetVeterinarianByIdAsync(int id);
        Task<List<VetSchedule>> GetVetScheduleAsync(int vetId);
        Task<List<AvailableSlot>> GetAvailabilityAsync(int vetId, DateTime date);
        Task<(bool success, string message, VetAppointment? appointment)> BookAppointmentAsync(int vetId, DateTime dateTime, string reason, string? notes = null, int? petId = null);
        Task<List<VetAppointment>> GetMyAppointmentsAsync();
        Task<(bool success, string message)> CancelAppointmentAsync(int appointmentId);
        Task<(bool success, string message)> ConfirmAppointmentAsync(int appointmentId); // For vets

        // Vet Portal Methods
        Task<(bool success, string? imageUrl, string? error)> UploadPhotoAsync(int vetId, Stream imageStream, string fileName);
        Task<(bool success, string? error)> UpdateProfileAsync(int vetId, string name, string specialty, string description, string phone, decimal price);
        Task<List<VetSchedule>> GetSchedulesAsync(int vetId);
        Task<(bool success, string? error)> CreateScheduleAsync(int vetId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int slotDuration);
        Task<bool> DeleteScheduleAsync(int scheduleId);

        // Vet Portal Appointments
        Task<List<VetAppointment>> GetVetAppointmentsAsync(int vetId);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
    }
}
