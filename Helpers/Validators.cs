using System.Text.RegularExpressions;

namespace FrontEndHealthPets.Helpers
{
    /// <summary>
    /// Input validation helpers
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Validates email format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates password strength
        /// Minimum 8 characters, at least one letter and one number
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);

            return hasLetter && hasDigit;
        }

        /// <summary>
        /// Gets password strength message
        /// </summary>
        public static string GetPasswordStrengthMessage(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Ingresa una contraseña";

            if (password.Length < 8)
                return "Mínimo 8 caracteres";

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            int strength = 0;
            if (hasLetter) strength++;
            if (hasDigit) strength++;
            if (hasUpper) strength++;
            if (hasSpecial) strength++;
            if (password.Length >= 12) strength++;

            return strength switch
            {
                0 or 1 => "Débil",
                2 or 3 => "Media",
                4 or 5 => "Fuerte",
                _ => "Muy fuerte"
            };
        }

        /// <summary>
        /// Validates required field
        /// </summary>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validates minimum length
        /// </summary>
        public static bool HasMinLength(string value, int minLength)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length >= minLength;
        }

        /// <summary>
        /// Validates maximum length
        /// </summary>
        public static bool HasMaxLength(string value, int maxLength)
        {
            return string.IsNullOrWhiteSpace(value) || value.Length <= maxLength;
        }

        /// <summary>
        /// Validates phone number (basic)
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            return digitsOnly.Length >= 8 && digitsOnly.Length <= 15;
        }

        /// <summary>
        /// Validates pet name
        /// </summary>
        public static bool IsValidPetName(string name)
        {
            return IsNotEmpty(name) && HasMinLength(name, 2) && HasMaxLength(name, 50);
        }

        /// <summary>
        /// Validates pet age
        /// </summary>
        public static bool IsValidPetAge(int age)
        {
            return age >= 0 && age <= 30;
        }

        /// <summary>
        /// Validates pet weight (in kg)
        /// </summary>
        public static bool IsValidPetWeight(decimal weight)
        {
            return weight > 0 && weight <= 200;
        }
    }
}
