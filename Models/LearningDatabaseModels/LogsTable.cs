using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning_Backend.Models.LearningDatabaseModels
{
    public class LogsTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataType("Bigint")]
        public int Id { get; set; }

        [Required]
        [DataType("Varchar(50)")]
        public string LogType { get; set; }
        [Required]
        [DataType("Varchar(50)")]
        public string LogMessage { get; set; }

        [Required]
        [DataType("Varchar(50)")]
        public string LogSource { get; set; }

        [Required]
        [DataType("DateTime")]
        public DateTime LogDate { get; set; }
    }
}
