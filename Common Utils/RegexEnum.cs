namespace Learning_Backend.Common_Utils
{
    public class RegexHelper
    {
        public enum RegexType
        {
            Email,
            Password,
            Username
        }

        public static string GetRegex(RegexType regexType) => regexType switch
        {
            RegexType.Email => @"^([\w\.\-]+)@(gmail\.com|nic\.com|yahoo\.com|thakur\.com)$",
            RegexType.Password => @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            RegexType.Username => @"^[a-zA-Z0-9]{5,15}$",
            _ => string.Empty
        };

        public static string GetErrorMessage(RegexType regexType) => regexType switch
        {
            RegexType.Email => "Invalid email format.",
            RegexType.Password => "Password must be 8-15 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.",
            RegexType.Username => "Username must be 5-15 characters long and contain only letters and digits.",
            _ => string.Empty
        };

        public static bool IsValid(RegexType regexType, string input) =>
            !string.IsNullOrEmpty(GetRegex(regexType)) && System.Text.RegularExpressions.Regex.IsMatch(input, GetRegex(regexType));

        public static string GetValidationError(RegexType regexType, string input) =>
            IsValid(regexType, input) ? string.Empty : GetErrorMessage(regexType);

        public static bool IsValidEmail(string email) => IsValid(RegexType.Email, email);

        public static string GetEmailErrorMessage(string email) => GetValidationError(RegexType.Email, email);

        public static bool IsValidPassword(string password) => IsValid(RegexType.Password, password);

        public static string GetPasswordErrorMessage(string password) => GetValidationError(RegexType.Password, password);

        public static bool IsValidUsername(string username) => IsValid(RegexType.Username, username);

        public static string GetUsernameErrorMessage(string username) => GetValidationError(RegexType.Username, username);
    }
}
