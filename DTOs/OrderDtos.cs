using System.ComponentModel.DataAnnotations;

namespace DBTest_BACK.DTOs
{
    // ============================================
    // DTOs DE PEDIDOS (ORDERS)
    // ============================================

    /// <summary>
    /// DTO para crear un nuevo pedido
    /// </summary>
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "El ID del cliente es requerido")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Los items son requeridos")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un item")]
        public List<OrderItemCreateDto> Items { get; set; } = new();

        [Required(ErrorMessage = "La dirección de envío es requerida")]
        public ShippingAddressDto ShippingAddress { get; set; } = new();

        [Required(ErrorMessage = "El método de pago es requerido")]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "credit_card";

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO para un item del pedido al crear
    /// </summary>
    public class OrderItemCreateDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ProductImage { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// DTO para dirección de envío
    /// </summary>
    public class ShippingAddressDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "El teléfono no es válido")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "La calle es requerida")]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es requerida")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código postal es requerido")]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        [MaxLength(100)]
        public string Country { get; set; } = "México";
    }

    /// <summary>
    /// DTO de respuesta completa de un pedido
    /// </summary>
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public ShippingAddressDto? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta de un item del pedido
    /// </summary>
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// DTO para lista de pedidos (sin items)
    /// </summary>
    public class OrderListDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }

    /// <summary>
    /// DTO para actualizar estado del pedido
    /// </summary>
    public class OrderStatusUpdateDto
    {
        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        [MaxLength(100)]
        public string? TrackingNumber { get; set; }
    }

    /// <summary>
    /// DTO para actualizar datos de un pedido (dirección y notas)
    /// </summary>
    public class OrderUpdateDto
    {
        [Required(ErrorMessage = "La dirección de envío es requerida")]
        public ShippingAddressDto ShippingAddress { get; set; } = new();

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO para filtros de búsqueda de pedidos
    /// </summary>
    public class OrderFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; }
        public string? Search { get; set; } // Buscar por OrderNumber o CustomerName
        public int? CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "CreatedAt"; // CreatedAt, Total, Status
        public bool SortDescending { get; set; } = true;
    }

    /// <summary>
    /// DTO para estadísticas de pedidos
    /// </summary>
    public class OrderStatsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
