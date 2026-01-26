
using System.Text.Json.Serialization;

namespace FrontEndHealthPets.Models
{
    public class VetAppointment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("veterinarianId")]
        public int VeterinarianId { get; set; }

        [JsonPropertyName("veterinarianName")]
        public string VeterinarianName { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("petId")]
        public int? PetId { get; set; }

        [JsonPropertyName("petName")]
        public string? PetName { get; set; }

        [JsonPropertyName("appointmentDateTime")]
        public DateTime AppointmentDateTime { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Helper for status color
        public string StatusColor => Status switch
        {
            "Confirmed" => "#4CAF50", // Green
            "Cancelled" => "#F44336", // Red
            "Completed" => "#2196F3", // Blue
            _ => "#FF9800"            // Orange (Pending)
        };
    }

    public class AvailableSlot
    {
        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }

        public string TimeDisplay => DateTime.ToString("hh:mm tt");
    }
}
