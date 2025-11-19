namespace DBTest_BACK.DTOs
{
    // Dashboard Statistics
    public class DashboardStatsDto
    {
        public SalesStat Sales { get; set; } = new();
        public OrdersStat Orders { get; set; } = new();
        public CustomersStat Customers { get; set; } = new();
        public ProductsStat Products { get; set; } = new();
    }

    public class SalesStat
    {
        public decimal Total { get; set; }
        public double Trend { get; set; }
        public string Label { get; set; } = "Ventas Totales";
    }

    public class OrdersStat
    {
        public int Total { get; set; }
        public double Trend { get; set; }
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }

    public class CustomersStat
    {
        public int Total { get; set; }
        public double Trend { get; set; }
        public int Active { get; set; }
    }

    public class ProductsStat
    {
        public int Total { get; set; }
        public double Trend { get; set; }
        public int InStock { get; set; }
        public int OutOfStock { get; set; }
    }

    // Order DTOs
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int Items { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrdersListResponse
    {
        public List<OrderDto> Orders { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Limit { get; set; }
    }

    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int Items { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CustomerInfo Customer { get; set; } = new();
        public ShippingAddressInfo ShippingAddress { get; set; } = new();
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public List<StatusHistoryDto> StatusHistory { get; set; } = new();
    }

    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }

    public class ShippingAddressInfo
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class StatusHistoryDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class UpdateOrderStatusResponse
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar dirección de envío y notas de un pedido
    /// </summary>
    public class UpdateOrderDto
    {
        public ShippingAddressUpdateDto ShippingAddress { get; set; } = new();
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO para actualizar dirección de envío
    /// </summary>
    public class ShippingAddressUpdateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "México";
    }

    /// <summary>
    /// DTO para cancelar un pedido
    /// </summary>
    public class CancelOrderDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta genérica para operaciones de pedidos
    /// </summary>
    public class OrderOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrderDetailDto? Data { get; set; }
    }

    // Top Products
    public class TopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
        public string? ImageUrl { get; set; }
    }

    // Activity Log
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    // Notifications
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UnreadCountDto
    {
        public int Count { get; set; }
    }

    // Paginación
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
    }

    // Chart Data
    public class ChartDataDto
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDatasetDto> Datasets { get; set; } = new();
    }

    public class ChartDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public List<decimal> Data { get; set; } = new();
        public List<string>? BackgroundColor { get; set; }
    }

    // User Management
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int OrdersCount { get; set; }
    }

    public class UpdateRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }
}
