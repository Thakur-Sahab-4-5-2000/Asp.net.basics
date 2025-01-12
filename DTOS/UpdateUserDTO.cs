using Learning_Backend.Common_Utils;
using System.ComponentModel.DataAnnotations;

namespace Learning_Backend.DTOS
{
    public class UpdateUserDTO
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your current password")]
        public string OldPassword { get; set; }

        public string? Password { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public void Validate()
        {
            List<string> errors = new List<string>();

            if (Email != null)
            {
                if (!RegexHelper.IsValidEmail(Email))
                {
                    errors.Add(RegexHelper.GetEmailErrorMessage(Email));
                }
            }

            if (string.IsNullOrEmpty(OldPassword))
            {
                errors.Add("Please enter your current password.");
            }

            if (Password != null)
            {
                if (!RegexHelper.IsValidUsername(Password))
                {
                    errors.Add(RegexHelper.GetPasswordErrorMessage(Password));
                }
            }

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join("; ", errors));
            }
        }
    }
}
