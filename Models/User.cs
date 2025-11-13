namespace BOSKOBACK.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string? Provider { get; set; } = "Local";
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        
        // Relationships
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
