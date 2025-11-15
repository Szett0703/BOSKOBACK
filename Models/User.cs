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
        
        // Role relationship
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        
        // Relationships
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
