using System;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Vaccine record model
    /// </summary>
    public class VaccineRecord
    {
        public long Id { get; set; }
        public long PetId { get; set; }
        public string VaccineName { get; set; } = string.Empty;
        public DateTime DateAdministered { get; set; }
        public int Dose { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string VeterinarianName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty; // PDF/Image of vaccine certificate

        // Computed properties
        public string StatusDisplay
        {
            get
            {
                if (!NextDueDate.HasValue)
                    return "Completada";

                var daysUntilDue = (NextDueDate.Value - DateTime.Now).Days;

                if (daysUntilDue < 0)
                    return "Vencida";
                else if (daysUntilDue <= 30)
                    return "Próxima";
                else
                    return "Al día";
            }
        }

        public string StatusColor
        {
            get
            {
                var status = StatusDisplay;
                return status switch
                {
                    "Vencida" => "#FF5252",
                    "Próxima" => "#FFB800",
                    "Al día" => "#00D9A5",
                    _ => "#636E72"
                };
            }
        }

        public int? DaysUntilDue
        {
            get
            {
                if (!NextDueDate.HasValue)
                    return null;

                return (NextDueDate.Value - DateTime.Now).Days;
            }
        }

        public string FormattedDate => DateAdministered.ToString("dd/MM/yyyy");

        public string NextDueDateFormatted => NextDueDate?.ToString("dd/MM/yyyy") ?? "N/A";

        public bool IsOverdue => NextDueDate.HasValue && NextDueDate.Value < DateTime.Now;

        public bool IsUpcoming => DaysUntilDue.HasValue && DaysUntilDue.Value <= 30 && DaysUntilDue.Value >= 0;

        public string DaysUntilDueDisplay
        {
            get
            {
                if (!DaysUntilDue.HasValue)
                    return "N/A";

                if (DaysUntilDue.Value < 0)
                    return $"Vencida hace {Math.Abs(DaysUntilDue.Value)} días";
                else if (DaysUntilDue.Value == 0)
                    return "Vence hoy";
                else if (DaysUntilDue.Value == 1)
                    return "Vence mañana";
                else
                    return $"Vence en {DaysUntilDue.Value} días";
            }
        }
    }
}
