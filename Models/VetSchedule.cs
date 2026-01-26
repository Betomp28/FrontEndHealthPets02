
using System.Text.Json.Serialization;

namespace FrontEndHealthPets.Models
{
    public class VetSchedule
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("veterinarianId")]
        public int VeterinarianId { get; set; }

        [JsonPropertyName("dayOfWeek")]
        public int DayOfWeek { get; set; }

        [JsonPropertyName("dayName")]
        public string DayName { get; set; }

        [JsonPropertyName("startTime")]
        public TimeSpan StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public TimeSpan EndTime { get; set; }

        [JsonPropertyName("slotDurationMinutes")]
        public int SlotDurationMinutes { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        // Helper property for display
        public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}
