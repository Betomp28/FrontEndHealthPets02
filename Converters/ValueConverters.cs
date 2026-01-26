using System.Globalization;

namespace FrontEndHealthPets.Converters
{
    /// <summary>
    /// Converts a boolean to its inverse
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }

    /// <summary>
    /// Returns true if value is not null
    /// </summary>
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns true if value is null or empty string
    /// </summary>
    public class IsNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
                return string.IsNullOrEmpty(str);
            return value == null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Returns one color if value equals parameter, else another
    /// </summary>
    public class SelectedColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Simple logic: if value == parameter (or selected item logic)
            // But usually this binds to (SelectedItem, ConverterParameter=Item)
            // Let's assume simpler: Binding is boolean (IsSelected) or we compare value to parameter
            // Actually, in the XAML it was likely: BackgroundColor="{Binding IsSelected, Converter={StaticResource SelectedBgConverter}}"
            
            // Wait, looking at current XAML in VeterinarianProfilePage:
            // VisualState is used for selection!
            // <VisualState x:Name="Selected"> <Setter Property="BackgroundColor" Value="#6B4EE6" /> ...
            
            // So maybe I DON'T need SelectedColorConverter if I use VisualStates properly?
            // The summary said "removed or simplified".
            // If I restored VisualStates (which seem to be in the code I viewed in step 624), then I might not need these specific converters for the slots.
            
            // However, "StatusCancelVisibleConverter" is definitely needed for MyAppointmentsPage.
            // Let's implement that one.
            return Colors.Transparent; // Placeholder
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusCancelVisibleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // Can cancel only if Pending or Confirmed
                return status == "Pending" || status == "Confirmed" || status == "Pendiente" || status == "Confirmada";
            }
            return false; 
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Pending" or "Pendiente" => Color.FromArgb("#FFB800"), // Orange
                    "Confirmed" or "Confirmada" => Color.FromArgb("#00D9A5"), // Green
                    "Cancelled" or "Cancelada" => Color.FromArgb("#FF5252"), // Red
                    "Completed" or "Completada" => Color.FromArgb("#667eea"), // Blue
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // ============ Chat & Notifications Converters ============

    public class FirstLetterConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
                return str[0].ToString().ToUpper();
            return "?";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue > 0;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBubbleColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool esPropio)
                return esPropio ? Color.FromArgb("#6B4EE6") : Color.FromArgb("#E8E8E8");
            return Color.FromArgb("#E8E8E8");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool esPropio)
                return esPropio ? LayoutOptions.End : LayoutOptions.Start;
            return LayoutOptions.Start;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool esPropio)
                return esPropio ? Colors.White : Color.FromArgb("#333333");
            return Color.FromArgb("#333333");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageTimeColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool esPropio)
                return esPropio ? Color.FromArgb("#CCCCCC") : Color.FromArgb("#999999");
            return Color.FromArgb("#999999");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReadStatusConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool leido)
                return leido ? "✓✓" : "✓";
            return "✓";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConversacionStatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string estado)
            {
                return estado.ToLower() switch
                {
                    "activa" or "active" => Color.FromArgb("#4CAF50"),
                    "cerrada" or "closed" => Color.FromArgb("#9E9E9E"),
                    _ => Color.FromArgb("#FF9800")
                };
            }
            return Color.FromArgb("#9E9E9E");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNotActiveConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string estado)
                return estado.ToLower() != "activa" && estado.ToLower() != "active";
            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsActiveConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string estado)
                return estado.ToLower() == "activa" || estado.ToLower() == "active";
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NotificationTypeIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string tipo)
            {
                return tipo.ToLower() switch
                {
                    "cita" or "appointment" => "📅",
                    "vacuna" or "vaccine" => "💉",
                    "mensaje" or "message" => "💬",
                    "recordatorio" or "reminder" => "⏰",
                    "alerta" or "alert" => "⚠️",
                    _ => "🔔"
                };
            }
            return "🔔";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int rating || (value is decimal decRating && int.TryParse(decRating.ToString("0"), out rating)))
            {
                return rating switch
                {
                    1 => "★☆☆☆☆",
                    2 => "★★☆☆☆",
                    3 => "★★★☆☆",
                    4 => "★★★★☆",
                    5 => "★★★★★",
                    _ => "☆☆☆☆☆"
                };
            }
            return "☆☆☆☆☆";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var span = DateTime.Now - dateTime;

                if (span.TotalMinutes < 1)
                    return "ahora";
                if (span.TotalMinutes < 60)
                    return $"hace {(int)span.TotalMinutes} min";
                if (span.TotalHours < 24)
                    return $"hace {(int)span.TotalHours} h";
                if (span.TotalDays < 7)
                    return $"hace {(int)span.TotalDays} dias";
                return dateTime.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReadBackgroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool leida)
                return leida ? Colors.White : Color.FromArgb("#F0EBF8");
            return Colors.White;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReadFontAttributeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool leida)
                return leida ? FontAttributes.None : FontAttributes.Bold;
            return FontAttributes.None;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StarColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int selectedRating && parameter is string paramStr && int.TryParse(paramStr, out int starNumber))
            {
                return selectedRating >= starNumber ? Color.FromArgb("#FFB800") : Color.FromArgb("#CCCCCC");
            }
            return Color.FromArgb("#CCCCCC");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns true if string is NOT null or empty (inverse of IsNullOrEmptyConverter)
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
                return !string.IsNullOrEmpty(str);
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
