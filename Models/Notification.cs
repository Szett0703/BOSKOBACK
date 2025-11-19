using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DBTest_BACK.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // order, product, user, system

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación - Ignorar para evitar ciclos
        [JsonIgnore]
        public User? User { get; set; }
    }
}
