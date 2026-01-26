namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Notification service interface
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Schedule local notification
        /// </summary>
        Task ScheduleNotificationAsync(int id, string title, string message, DateTime scheduleTime);

        /// <summary>
        /// Cancel scheduled notification
        /// </summary>
        Task CancelNotificationAsync(int id);

        /// <summary>
        /// Cancel all notifications
        /// </summary>
        Task CancelAllNotificationsAsync();

        /// <summary>
        /// Request notification permissions
        /// </summary>
        Task<bool> RequestPermissionsAsync();

        /// <summary>
        /// Check if notifications are enabled
        /// </summary>
        Task<bool> AreNotificationsEnabledAsync();

        /// <summary>
        /// Schedule vaccine reminder
        /// </summary>
        Task ScheduleVaccineReminderAsync(long vaccineId, string petName, string vaccineName, DateTime dueDate);

        /// <summary>
        /// Schedule appointment reminder
        /// </summary>
        Task ScheduleAppointmentReminderAsync(long appointmentId, string petName, DateTime appointmentDate);

        /// <summary>
        /// Schedule medication reminder
        /// </summary>
        /// <summary>
        /// Cancel all notifications for a specific pet
        /// </summary>
        Task CancelAllPetNotificationsAsync(long petId);
    }
}
