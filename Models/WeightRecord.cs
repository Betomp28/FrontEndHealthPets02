using System;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Registro de peso de una mascota
    /// </summary>
    public class WeightRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long PetId { get; set; }
        public double WeightKg { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }

        /// <summary>
        /// Peso formateado para mostrar en UI
        /// </summary>
        public string WeightFormatted => $"{WeightKg:F1} kg";

        /// <summary>
        /// Fecha formateada corta
        /// </summary>
        public string DateFormatted => RecordDate.ToString("dd/MM/yyyy");

        /// <summary>
        /// Fecha formateada para gráfica (día/mes)
        /// </summary>
        public string DateShort => RecordDate.ToString("dd/MM");
    }

    /// <summary>
    /// Estadísticas de peso calculadas
    /// </summary>
    public class WeightStats
    {
        public double CurrentWeight { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public double AverageWeight { get; set; }
        public double WeightChange { get; set; } // Diferencia con el registro anterior
        public string WeightTrend { get; set; } = "stable"; // up, down, stable

        public string CurrentWeightFormatted => $"{CurrentWeight:F1} kg";
        public string MinWeightFormatted => $"{MinWeight:F1} kg";
        public string MaxWeightFormatted => $"{MaxWeight:F1} kg";
        public string AverageWeightFormatted => $"{AverageWeight:F1} kg";

        public string WeightChangeFormatted
        {
            get
            {
                if (WeightChange > 0)
                    return $"+{WeightChange:F1} kg";
                else if (WeightChange < 0)
                    return $"{WeightChange:F1} kg";
                return "0 kg";
            }
        }

        public string TrendIcon
        {
            get
            {
                return WeightTrend switch
                {
                    "up" => "📈",
                    "down" => "📉",
                    _ => "➡️"
                };
            }
        }

        public Color TrendColor
        {
            get
            {
                return WeightTrend switch
                {
                    "up" => Color.FromArgb("#FF9800"),    // Naranja (subió)
                    "down" => Color.FromArgb("#4CAF50"),  // Verde (bajó - generalmente bueno)
                    _ => Color.FromArgb("#2196F3")        // Azul (estable)
                };
            }
        }
    }
}
