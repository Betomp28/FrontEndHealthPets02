using System;

namespace FrontEndHealthPets.Models
{
    public class Appointment
    {
        public long Id { get; set; }
        public long PetId { get; set; }
        public long ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string VeterinarianName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pendiente"; // Pendiente, Confirmada, Completada, Cancelada
        public string Notes { get; set; } = string.Empty;
        public bool ReminderSent { get; set; }

        // Computed properties
        public DateTime DateTime => Date.Add(Time);

        public string DateTimeDisplay => $"{Date:dd/MM/yyyy} a las {Time:hh\\:mm tt}";

        public string TimeDisplay => Time.ToString(@"hh\:mm");

        public string DayNumber => Date.Day.ToString();

        public string MonthShort => Date.ToString("MMM").ToUpper();

        public string StatusColor
        {
            get
            {
                return Status switch
                {
                    "Confirmada" => "#00D9A5",
                    "Pendiente" => "#FFB800",
                    "Completada" => "#636E72",
                    "Cancelada" => "#FF5252",
                    _ => "#B2BEC3"
                };
            }
        }

        public bool IsPast => DateTime < System.DateTime.Now;

        public bool IsToday => Date.Date == System.DateTime.Today;

        public bool IsUpcoming => DateTime > System.DateTime.Now;

        public int DaysUntil => (Date.Date - System.DateTime.Today).Days;

        public string DaysUntilDisplay
        {
            get
            {
                if (IsToday) return "Hoy";
                if (DaysUntil == 1) return "Mañana";
                if (DaysUntil == -1) return "Ayer";
                if (DaysUntil < 0) return $"Hace {Math.Abs(DaysUntil)} días";
                return $"En {DaysUntil} días";
            }
        }

        public string FullDisplay => $"{ClinicName} - {Reason} ({DateTimeDisplay})";
    }
}
