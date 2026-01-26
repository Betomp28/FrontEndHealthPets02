using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Representa una consulta/historial clínico
    /// </summary>
    public class Consulta : INotifyPropertyChanged
    {
        private int _id;
        private int _petId;
        private string _petName = string.Empty;
        private string _petSpecies = string.Empty;
        private int _veterinarianId;
        private string _veterinarianName = string.Empty;
        private string? _clinicName;
        private string? _reasonForVisit;
        private string? _symptoms;
        private string? _diagnosis;
        private string? _treatment;
        private string? _observations;
        private string? _recommendations;
        private decimal? _currentWeight;
        private decimal? _temperature;
        private int? _heartRate;
        private int? _respiratoryRate;
        private string _consultationType = "GENERAL";
        private bool _isEmergency;
        private DateTime _consultationDate;
        private DateTime? _nextCheckupDate;
        private decimal? _consultationCost;
        private bool _isPaid;
        private List<ConsultaArchivo> _attachments = new();

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int PetId
        {
            get => _petId;
            set { _petId = value; OnPropertyChanged(); }
        }

        public string PetName
        {
            get => _petName;
            set { _petName = value; OnPropertyChanged(); }
        }

        public string PetSpecies
        {
            get => _petSpecies;
            set { _petSpecies = value; OnPropertyChanged(); }
        }

        public int VeterinarianId
        {
            get => _veterinarianId;
            set { _veterinarianId = value; OnPropertyChanged(); }
        }

        public string VeterinarianName
        {
            get => _veterinarianName;
            set { _veterinarianName = value; OnPropertyChanged(); }
        }

        public string? ClinicName
        {
            get => _clinicName;
            set { _clinicName = value; OnPropertyChanged(); }
        }

        public string? ReasonForVisit
        {
            get => _reasonForVisit;
            set { _reasonForVisit = value; OnPropertyChanged(); }
        }

        public string? Symptoms
        {
            get => _symptoms;
            set { _symptoms = value; OnPropertyChanged(); }
        }

        public string? Diagnosis
        {
            get => _diagnosis;
            set { _diagnosis = value; OnPropertyChanged(); }
        }

        public string? Treatment
        {
            get => _treatment;
            set { _treatment = value; OnPropertyChanged(); }
        }

        public string? Observations
        {
            get => _observations;
            set { _observations = value; OnPropertyChanged(); }
        }

        public string? Recommendations
        {
            get => _recommendations;
            set { _recommendations = value; OnPropertyChanged(); }
        }

        public decimal? CurrentWeight
        {
            get => _currentWeight;
            set { _currentWeight = value; OnPropertyChanged(); }
        }

        public decimal? Temperature
        {
            get => _temperature;
            set { _temperature = value; OnPropertyChanged(); }
        }

        public int? HeartRate
        {
            get => _heartRate;
            set { _heartRate = value; OnPropertyChanged(); }
        }

        public int? RespiratoryRate
        {
            get => _respiratoryRate;
            set { _respiratoryRate = value; OnPropertyChanged(); }
        }

        public string ConsultationType
        {
            get => _consultationType;
            set { _consultationType = value; OnPropertyChanged(); }
        }

        public bool IsEmergency
        {
            get => _isEmergency;
            set { _isEmergency = value; OnPropertyChanged(); }
        }

        public DateTime ConsultationDate
        {
            get => _consultationDate;
            set { _consultationDate = value; OnPropertyChanged(); }
        }

        public DateTime? NextCheckupDate
        {
            get => _nextCheckupDate;
            set { _nextCheckupDate = value; OnPropertyChanged(); }
        }

        public decimal? ConsultationCost
        {
            get => _consultationCost;
            set { _consultationCost = value; OnPropertyChanged(); }
        }

        public bool IsPaid
        {
            get => _isPaid;
            set { _isPaid = value; OnPropertyChanged(); }
        }

        public List<ConsultaArchivo> Attachments
        {
            get => _attachments;
            set { _attachments = value; OnPropertyChanged(); }
        }

        // Alias properties for backward compatibility (Spanish names)
        public DateTime FechaConsulta
        {
            get => ConsultationDate;
            set => ConsultationDate = value;
        }

        public string VeterinarioNombre
        {
            get => VeterinarianName;
            set => VeterinarianName = value;
        }

        public string TipoConsulta
        {
            get => ConsultationType;
            set => ConsultationType = value;
        }

        public string? MotivoConsulta
        {
            get => ReasonForVisit;
            set => ReasonForVisit = value;
        }

        public string? Diagnostico
        {
            get => Diagnosis;
            set => Diagnosis = value;
        }

        public string? Tratamiento
        {
            get => Treatment;
            set => Treatment = value;
        }

        public string? Notas
        {
            get => Observations;
            set => Observations = value;
        }

        // Propiedades calculadas
        public string ConsultationDateFormatted => ConsultationDate.ToString("dd/MM/yyyy HH:mm");
        public string NextCheckupFormatted => NextCheckupDate?.ToString("dd/MM/yyyy") ?? "No programada";
        public string WeightFormatted => CurrentWeight.HasValue ? $"{CurrentWeight:F1} kg" : "-";
        public string TemperatureFormatted => Temperature.HasValue ? $"{Temperature:F1} °C" : "-";
        public string CostFormatted => ConsultationCost.HasValue ? $"${ConsultationCost:F2}" : "-";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Archivo adjunto a una consulta
    /// </summary>
    public class ConsultaArchivo
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? FileSizeKb { get; set; }
        public DateTime UploadDate { get; set; }

        public string FileSizeFormatted => FileSizeKb.HasValue ? $"{FileSizeKb} KB" : "";
    }

    /// <summary>
    /// Historial clínico completo de una mascota
    /// </summary>
    public class HistorialClinico
    {
        public Pet Pet { get; set; } = new();
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public List<Consulta> Consultations { get; set; } = new();
        public List<VaccineRecord> Vaccines { get; set; } = new();
        public List<MedicationRecord> Medications { get; set; } = new();
    }

    /// <summary>
    /// Registro de medicamento
    /// </summary>
    public class MedicationRecord
    {
        public int Id { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? AdministrationMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
    }
}
