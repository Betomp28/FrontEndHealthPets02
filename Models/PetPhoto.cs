using System;

namespace FrontEndHealthPets.Models
{
    public class PetPhoto
    {
        public long Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string Caption { get; set; } = string.Empty;
    }
}
