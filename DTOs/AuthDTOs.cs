using System.ComponentModel.DataAnnotations;

namespace BOSKOBACK.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!;
    }

    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Token { get; set; } = null!;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
