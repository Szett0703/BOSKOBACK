using System.ComponentModel.DataAnnotations;

namespace DBTest_BACK.DTOs
{
    public class CreateAddressDto
    {
        [Required(ErrorMessage = "La etiqueta es requerida")]
        [StringLength(100, ErrorMessage = "La etiqueta no puede exceder 100 caracteres")]
        public string Label { get; set; } = string.Empty;

        [Required(ErrorMessage = "La calle es requerida")]
        [StringLength(200, ErrorMessage = "La calle no puede exceder 200 caracteres")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es requerida")]
        [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres")]
        public string? State { get; set; }

        [Required(ErrorMessage = "El código postal es requerido")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "El código postal debe tener entre 4 y 20 caracteres")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
        public string Country { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Phone { get; set; }

        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressDto
    {
        [Required(ErrorMessage = "La etiqueta es requerida")]
        [StringLength(100)]
        public string Label { get; set; } = string.Empty;

        [Required(ErrorMessage = "La calle es requerida")]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es requerida")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(100)]
        public string? State { get; set; }

        [Required(ErrorMessage = "El código postal es requerido")]
        [StringLength(20, MinimumLength = 4)]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        public bool IsDefault { get; set; }
    }

    public class AddressResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Label { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
