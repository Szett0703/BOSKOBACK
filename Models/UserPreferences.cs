using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBTest_BACK.Models
{
    [Table("UserPreferences")]
    public class UserPreferences
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public bool Notifications { get; set; } = true;

        [Required]
        public bool Newsletter { get; set; } = true;

        [MaxLength(10)]
        public string Language { get; set; } = "es";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
