using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DBTest_BACK.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Relación - Ignorar para evitar ciclos
        [JsonIgnore]
        public Order? Order { get; set; }
    }
}
