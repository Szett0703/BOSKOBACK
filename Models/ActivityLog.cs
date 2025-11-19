using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DBTest_BACK.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // order, product, user, category

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Relación - Ignorar para evitar ciclos
        [JsonIgnore]
        public User? User { get; set; }
    }
}
