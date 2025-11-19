using System.ComponentModel.DataAnnotations;

namespace DBTest_BACK.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public string? AvatarUrl { get; set; }
        public UserPreferencesDto? Preferences { get; set; }
    }

    public class UserPreferencesDto
    {
        public bool Notifications { get; set; }
        public bool Newsletter { get; set; }
        public string? Language { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no es válido")]
        public string? Phone { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdatePreferencesDto
    {
        public bool Notifications { get; set; }
        public bool Newsletter { get; set; }
        public string? Language { get; set; }
    }
}
