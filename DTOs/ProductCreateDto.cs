using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DBTest_BACK.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [JsonPropertyName("stock")]
        public int Stock { get; set; }

        [MaxLength(500)]
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }
    }
}
