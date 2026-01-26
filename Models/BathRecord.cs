using System;

namespace FrontEndHealthPets.Models
{
    public class BathRecord
    {
        public int Id { get; set; }
        public long PetId { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string GroomerName { get; set; } // Nombre del lugar o persona
        public DateTime? NextBathDate { get; set; }
        
        // Helper property for display
        public string DateDisplay => Date.ToString("dd/MM/yyyy");
        public string NextBathDisplay => NextBathDate.HasValue ? NextBathDate.Value.ToString("dd/MM/yyyy") : "No programado";
    }
}
