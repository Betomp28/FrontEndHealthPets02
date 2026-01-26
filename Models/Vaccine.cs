using System;

namespace FrontEndHealthPets.Models
{
    public enum VaccineStatus
    {
        Completed,
        Upcoming,
        Overdue
    }

    public class Vaccine
    {
        public long PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateAdministered { get; set; }
        public DateTime NextDueDate { get; set; }
        public VaccineStatus Status { get; set; }
    }
}
