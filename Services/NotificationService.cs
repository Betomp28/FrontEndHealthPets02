using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    public class NotificationService : INotificationService
    {
        private bool _notificationsEnabled = false;

        public NotificationService()
        {
            _notificationsEnabled = Settings.NotificationsEnabled;
        }

        public async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                // TODO: Implement platform-specific permission request
                await Task.Delay(100);
                _notificationsEnabled = true;
                Settings.NotificationsEnabled = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error requesting notification permission: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AreNotificationsEnabledAsync()
        {
            await Task.CompletedTask;
            return _notificationsEnabled && Settings.NotificationsEnabled;
        }

        public async Task ScheduleNotificationAsync(int id, string title, string message, DateTime scheduleTime)
        {
            if (!_notificationsEnabled)
            {
                Console.WriteLine("Notifications not enabled");
                return;
            }

            try
            {
                // TODO: Implement platform-specific notification scheduling
                Console.WriteLine($"Scheduled notification #{id}: {title} - {message} at {scheduleTime}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling notification: {ex.Message}");
            }
        }

        public async Task CancelNotificationAsync(int id)
        {
            try
            {
                // TODO: Implement platform-specific notification cancellation
                Console.WriteLine($"Cancelled notification #{id}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling notification: {ex.Message}");
            }
        }

        public async Task CancelAllNotificationsAsync()
        {
            try
            {
                // TODO: Implement platform-specific cancel all
                Console.WriteLine("Cancelled all notifications");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling all notifications: {ex.Message}");
            }
        }

        public async Task ScheduleVaccineReminderAsync(long vaccineId, string petName, string vaccineName, DateTime dueDate)
        {
            if (!_notificationsEnabled || !Settings.VaccineRemindersEnabled)
                return;

            try
            {
                // Schedule reminder 7 days before
                var reminderDate = dueDate.AddDays(-7);
                if (reminderDate > DateTime.Now)
                {
                    var notificationId = GenerateNotificationId(vaccineId, "vaccine");
                    var title = $"Recordatorio de Vacuna - {petName}";
                    var message = $"La vacuna '{vaccineName}' vence el {dueDate:dd/MM/yyyy}. No olvides vacunar a {petName}.";

                    await ScheduleNotificationAsync(notificationId, title, message, reminderDate);
                }

                // Schedule reminder on the day
                if (dueDate.Date >= DateTime.Today)
                {
                    var notificationId = GenerateNotificationId(vaccineId, "vaccine_due");
                    var title = $"Vacuna Pendiente - {petName}";
                    var message = $"Hoy vence la vacuna '{vaccineName}' de {petName}.";
                    var scheduledTime = dueDate.Date.AddHours(9); // 9 AM

                    await ScheduleNotificationAsync(notificationId, title, message, scheduledTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling vaccine reminder: {ex.Message}");
            }
        }

        public async Task ScheduleAppointmentReminderAsync(long appointmentId, string petName, DateTime appointmentDate)
        {
            if (!_notificationsEnabled || !Settings.AppointmentRemindersEnabled)
                return;

            try
            {
                // Schedule reminder 24 hours before
                var reminderDate = appointmentDate.AddHours(-24);
                if (reminderDate > DateTime.Now)
                {
                    var notificationId = GenerateNotificationId(appointmentId, "appointment_24h");
                    var title = $"Recordatorio de Cita - {petName}";
                    var message = $"Mañana a las {appointmentDate:HH:mm} tienes una cita con {petName}.";

                    await ScheduleNotificationAsync(notificationId, title, message, reminderDate);
                }

                // Schedule reminder 1 hour before
                var oneHourBefore = appointmentDate.AddHours(-1);
                if (oneHourBefore > DateTime.Now)
                {
                    var notificationId = GenerateNotificationId(appointmentId, "appointment_1h");
                    var title = $"Cita Próxima - {petName}";
                    var message = $"En 1 hora: Cita con {petName}.";

                    await ScheduleNotificationAsync(notificationId, title, message, oneHourBefore);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling appointment reminder: {ex.Message}");
            }
        }

        public async Task ScheduleMedicationReminderAsync(long medicationId, string petName, string medicationName, DateTime reminderTime)
        {
            if (!_notificationsEnabled || !Settings.MedicationRemindersEnabled)
                return;

            try
            {
                if (reminderTime > DateTime.Now)
                {
                    var notificationId = GenerateNotificationId(medicationId, "medication");
                    var title = $"Medicación - {petName}";
                    var message = $"Es hora de darle {medicationName} a {petName}.";

                    await ScheduleNotificationAsync(notificationId, title, message, reminderTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling medication reminder: {ex.Message}");
            }
        }

        public async Task CancelAllPetNotificationsAsync(long petId)
        {
            try
            {
                // Cancel all notification types for this pet
                var types = new[] { "vaccine", "vaccine_due", "appointment_24h", "appointment_1h", "medication" };
                
                foreach (var type in types)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var notificationId = GenerateNotificationId(petId, $"{type}_{i}");
                        await CancelNotificationAsync(notificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling pet notifications: {ex.Message}");
            }
        }

        private int GenerateNotificationId(long entityId, string type)
        {
            var hash = $"{entityId}_{type}".GetHashCode();
            return Math.Abs(hash);
        }
    }
}
