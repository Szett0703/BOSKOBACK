using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBTest_BACK.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]  // Actualizado para coincidir con DB: nvarchar(150)
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]  // Actualizado para coincidir con DB: nvarchar(150)
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? PasswordHash { get; set; } // Null para usuarios de Google

        [MaxLength(50)]  // Actualizado para coincidir con DB: nvarchar(50)
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]  // Actualizado para coincidir con DB: nvarchar(50)
        public string Role { get; set; } = "Customer"; // Admin, Employee, Customer

        [Required]
        [MaxLength(50)]  // Actualizado para coincidir con DB: nvarchar(50)
        public string Provider { get; set; } = "Local"; // Local, Google

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;
    }

    [Table("PasswordResetTokens")]
    public class PasswordResetToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]  // Actualizado para coincidir con DB: nvarchar(255)
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsUsed { get; set; } = false;  // Agregar de vuelta

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
