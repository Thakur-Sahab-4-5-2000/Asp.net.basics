namespace Learning_Backend.Models.LearningDatabaseModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

      
        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(255)")]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(250)]
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Required]
        public int Role { get; set; }

        public string? ProfileImagePath { get; set; }
    }
}
