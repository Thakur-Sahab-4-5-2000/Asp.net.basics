using Learning_Backend.Common_Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Learning_Backend.DTOS
{
    public class RegisterUserDTO
    {
        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(250)]
        [Column(TypeName = "varchar(250)")]
        public string Email { get; set; }

        [Required]
        public int Role { get; set; }

        public IFormFile? ProfileImage { get; set; }

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

            if (string.IsNullOrEmpty(PasswordHash))
            {
                errors.Add("Password cannot be null or empty.");
            }
            else if (!RegexHelper.IsValidPassword(PasswordHash))
            {
                errors.Add(RegexHelper.GetPasswordErrorMessage(PasswordHash));
            }
            if (string.IsNullOrEmpty(Email))
            {
                errors.Add("Email cannot be null or empty.");
            }
            else if (!RegexHelper.IsValidEmail(Email))
            {
                errors.Add(RegexHelper.GetEmailErrorMessage(Email));
            }
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join("; ", errors));
            }
        }
    }
}
