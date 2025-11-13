namespace BOSKOBACK.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
