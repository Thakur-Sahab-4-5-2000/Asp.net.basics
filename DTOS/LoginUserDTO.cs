using Learning_Backend.Common_Utils;

namespace Learning_Backend.DTOS
{
    public class LoginUserDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public void Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(Username))
            {
                errors.Add("Username cannot be null or empty.");
            }
            else if (!RegexHelper.IsValidUsername(Username))
            {
                errors.Add(RegexHelper.GetUsernameErrorMessage(Username));
            }

            if (string.IsNullOrEmpty(Password))
            {
                errors.Add("Password cannot be null or empty.");
            }
            else if (!RegexHelper.IsValidPassword(Password))
            {
                errors.Add(RegexHelper.GetPasswordErrorMessage(Password));
            }

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join("; ", errors));
            }
        }
    }
}
