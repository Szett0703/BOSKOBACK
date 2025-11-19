using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IActivityLogService _activityLogService;

        public OrderService(
            AppDbContext context,
            ILogger<OrderService> logger,
            IActivityLogService activityLogService)
        {
            _context = context;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(OrderCreateDto dto)
        {
            try
            {
                // Validar que el cliente existe
                var customer = await _context.Users.FindAsync(dto.CustomerId);
                if (customer == null)
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse("Cliente no encontrado");
                }

                // Calcular totales
                decimal subtotal = 0;
                foreach (var item in dto.Items)
                {
                    subtotal += item.UnitPrice * item.Quantity;
                }

                decimal tax = subtotal * 0.16m; // 16% IVA
                decimal shippingCost = 100m; // Costo fijo de envío
                decimal total = subtotal + tax + shippingCost;

                // Crear la orden
                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    OrderNumber = GenerateOrderNumber(),
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email,
                    Subtotal = subtotal,
                    Tax = tax,
                    Shipping = shippingCost,
                    Total = total,
                    Status = "pending",
                    PaymentMethod = dto.PaymentMethod,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Agregar items
                foreach (var itemDto in dto.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        ProductImage = itemDto.ProductImage,
                        Quantity = itemDto.Quantity,
                        Price = itemDto.UnitPrice,
                        Subtotal = itemDto.UnitPrice * itemDto.Quantity
                    };

                    _context.OrderItems.Add(orderItem);

                    // Actualizar stock del producto
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product != null)
                    {
                        product.Stock -= itemDto.Quantity;
                    }
                }

                // Agregar dirección de envío
                var shippingAddress = new ShippingAddress
                {
                    OrderId = order.Id,
                    FullName = dto.ShippingAddress.FullName,
                    Phone = dto.ShippingAddress.Phone,
                    Street = dto.ShippingAddress.Street,
                    City = dto.ShippingAddress.City,
                    State = dto.ShippingAddress.State,
                    PostalCode = dto.ShippingAddress.PostalCode,
                    Country = dto.ShippingAddress.Country
                };

                _context.ShippingAddresses.Add(shippingAddress);

                // Agregar historial de estado
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = "pending",
                    Note = "Pedido creado",
                    Timestamp = DateTime.UtcNow
                };

                _context.OrderStatusHistory.Add(statusHistory);

                await _context.SaveChangesAsync();

                // Registrar actividad
                await _activityLogService.LogActivityAsync(
                    "order",
                    $"Nuevo pedido {order.OrderNumber} creado",
                    dto.CustomerId
                );

                // Cargar relaciones para la respuesta
                await _context.Entry(order).Collection(o => o.Items).LoadAsync();
                await _context.Entry(order).Reference(o => o.ShippingAddressDetails).LoadAsync();

                var response = MapToResponseDto(order);

                _logger.LogInformation("Pedido creado: {OrderNumber} (ID: {OrderId})", 
                    order.OrderNumber, order.Id);

                return ApiResponse<OrderResponseDto>.SuccessResponse(
                    response,
                    "Pedido creado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error al crear el pedido",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .Include(o => o.ShippingAddressDetails)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse(
                        $"Pedido con ID {id} no encontrado"
                    );
                }

                var response = MapToResponseDto(order);

                return ApiResponse<OrderResponseDto>.SuccessResponse(
                    response,
                    "Pedido obtenido exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido {OrderId}", id);
                return ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error al obtener el pedido",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<PagedResponse<OrderListDto>>> GetOrdersAsync(OrderFilterDto filters)
        {
            try
            {
                // Validar y ajustar parámetros
                if (filters.PageSize > 100) filters.PageSize = 100;
                if (filters.PageSize < 1) filters.PageSize = 10;
                if (filters.Page < 1) filters.Page = 1;

                var query = _context.Orders
                    .Include(o => o.Items)
                    .AsQueryable();

                // Filtro por estado
                if (!string.IsNullOrWhiteSpace(filters.Status))
                {
                    query = query.Where(o => o.Status == filters.Status.ToLower());
                }

                // Filtro por búsqueda (OrderNumber o CustomerName)
                if (!string.IsNullOrWhiteSpace(filters.Search))
                {
                    var searchLower = filters.Search.ToLower();
                    query = query.Where(o =>
                        (o.OrderNumber != null && o.OrderNumber.ToLower().Contains(searchLower)) ||
                        o.CustomerName.ToLower().Contains(searchLower)
                    );
                }

                // Filtro por cliente
                if (filters.CustomerId.HasValue)
                {
                    query = query.Where(o => o.CustomerId == filters.CustomerId.Value);
                }

                // Filtro por rango de fechas
                if (filters.StartDate.HasValue)
                {
                    query = query.Where(o => o.CreatedAt >= filters.StartDate.Value);
                }

                if (filters.EndDate.HasValue)
                {
                    query = query.Where(o => o.CreatedAt <= filters.EndDate.Value);
                }

                // Ordenamiento
                query = filters.SortBy.ToLower() switch
                {
                    "total" => filters.SortDescending
                        ? query.OrderByDescending(o => o.Total)
                        : query.OrderBy(o => o.Total),
                    "status" => filters.SortDescending
                        ? query.OrderByDescending(o => o.Status)
                        : query.OrderBy(o => o.Status),
                    _ => filters.SortDescending
                        ? query.OrderByDescending(o => o.CreatedAt)
                        : query.OrderBy(o => o.CreatedAt)
                };

                // Contar total antes de paginar
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var orders = await query
                    .Skip((filters.Page - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .Select(o => new OrderListDto
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber ?? string.Empty,
                        CustomerName = o.CustomerName,
                        Date = o.CreatedAt,
                        Status = o.Status,
                        Total = o.Total,
                        PaymentMethod = o.PaymentMethod,
                        ItemsCount = o.Items.Count
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize);

                var response = new PagedResponse<OrderListDto>
                {
                    Items = orders,
                    Page = filters.Page,
                    CurrentPage = filters.Page,
                    PageSize = filters.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

                return ApiResponse<PagedResponse<OrderListDto>>.SuccessResponse(
                    response,
                    "Pedidos obtenidos exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos");
                return ApiResponse<PagedResponse<OrderListDto>>.ErrorResponse(
                    "Error al obtener pedidos",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto>> UpdateOrderStatusAsync(int id, OrderStatusUpdateDto dto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .Include(o => o.ShippingAddressDetails)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse(
                        $"Pedido con ID {id} no encontrado"
                    );
                }

                // Validar estado
                var validStatuses = new[] { "pending", "processing", "delivered", "cancelled" };
                if (!validStatuses.Contains(dto.Status.ToLower()))
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse(
                        "Estado inválido. Valores permitidos: pending, processing, delivered, cancelled"
                    );
                }

                var previousStatus = order.Status;
                order.Status = dto.Status.ToLower();
                order.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
                {
                    order.TrackingNumber = dto.TrackingNumber;
                }

                // Agregar historial de estado
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = order.Status,
                    Note = dto.Note ?? $"Estado cambiado de {previousStatus} a {order.Status}",
                    Timestamp = DateTime.UtcNow
                };

                _context.OrderStatusHistory.Add(statusHistory);
                await _context.SaveChangesAsync();

                // Registrar actividad
                await _activityLogService.LogActivityAsync(
                    "order",
                    $"Pedido {order.OrderNumber} actualizado a {order.Status}",
                    null
                );

                var response = MapToResponseDto(order);

                _logger.LogInformation("Pedido {OrderNumber} actualizado a {Status}", 
                    order.OrderNumber, order.Status);

                return ApiResponse<OrderResponseDto>.SuccessResponse(
                    response,
                    "Estado del pedido actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido {OrderId}", id);
                return ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error al actualizar el estado del pedido",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto>> UpdateOrderAsync(int id, OrderUpdateDto dto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .Include(o => o.ShippingAddressDetails)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse(
                        $"Pedido con ID {id} no encontrado"
                    );
                }

                // Solo se pueden editar pedidos en estado "pending"
                if (order.Status.ToLower() != "pending")
                {
                    return ApiResponse<OrderResponseDto>.ErrorResponse(
                        "Solo puedes editar pedidos en estado Pendiente"
                    );
                }

                // Actualizar dirección de envío
                if (order.ShippingAddressDetails != null)
                {
                    order.ShippingAddressDetails.FullName = dto.ShippingAddress.FullName;
                    order.ShippingAddressDetails.Phone = dto.ShippingAddress.Phone;
                    order.ShippingAddressDetails.Street = dto.ShippingAddress.Street;
                    order.ShippingAddressDetails.City = dto.ShippingAddress.City;
                    order.ShippingAddressDetails.State = dto.ShippingAddress.State;
                    order.ShippingAddressDetails.PostalCode = dto.ShippingAddress.PostalCode;
                    order.ShippingAddressDetails.Country = dto.ShippingAddress.Country;
                }

                // Actualizar notas
                if (dto.Notes != null)
                {
                    order.Notes = dto.Notes;
                }

                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Registrar actividad
                await _activityLogService.LogActivityAsync(
                    "order",
                    $"Pedido {order.OrderNumber} actualizado (dirección/notas)",
                    null
                );

                var response = MapToResponseDto(order);

                _logger.LogInformation("Pedido {OrderNumber} actualizado", order.OrderNumber);

                return ApiResponse<OrderResponseDto>.SuccessResponse(
                    response,
                    "Pedido actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pedido {OrderId}", id);
                return ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error al actualizar el pedido",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int id, string reason)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Pedido con ID {id} no encontrado"
                    );
                }

                if (order.Status == "delivered")
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "No se puede cancelar un pedido ya entregado"
                    );
                }

                order.Status = "cancelled";
                order.UpdatedAt = DateTime.UtcNow;

                // Devolver stock a los productos
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }

                // Agregar historial
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = "cancelled",
                    Note = reason ?? "Pedido cancelado",
                    Timestamp = DateTime.UtcNow
                };

                _context.OrderStatusHistory.Add(statusHistory);
                await _context.SaveChangesAsync();

                // Registrar actividad
                await _activityLogService.LogActivityAsync(
                    "order",
                    $"Pedido {order.OrderNumber} cancelado: {reason}",
                    null
                );

                _logger.LogInformation("Pedido {OrderNumber} cancelado", order.OrderNumber);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Pedido cancelado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar pedido {OrderId}", id);
                return ApiResponse<bool>.ErrorResponse(
                    "Error al cancelar el pedido",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<List<OrderListDto>>> GetCustomerOrdersAsync(int customerId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Items)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new OrderListDto
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber ?? string.Empty,
                        CustomerName = o.CustomerName,
                        Date = o.CreatedAt,
                        Status = o.Status,
                        Total = o.Total,
                        PaymentMethod = o.PaymentMethod,
                        ItemsCount = o.Items.Count
                    })
                    .ToListAsync();

                return ApiResponse<List<OrderListDto>>.SuccessResponse(
                    orders,
                    "Pedidos del cliente obtenidos exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del cliente {CustomerId}", customerId);
                return ApiResponse<List<OrderListDto>>.ErrorResponse(
                    "Error al obtener los pedidos del cliente",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<OrderStatsDto>> GetOrderStatsAsync()
        {
            try
            {
                var totalOrders = await _context.Orders.CountAsync();
                var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "pending");
                var processingOrders = await _context.Orders.CountAsync(o => o.Status == "processing");
                var deliveredOrders = await _context.Orders.CountAsync(o => o.Status == "delivered");
                var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "cancelled");

                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == "delivered")
                    .SumAsync(o => o.Total);

                var averageOrderValue = deliveredOrders > 0 ? totalRevenue / deliveredOrders : 0;

                var stats = new OrderStatsDto
                {
                    TotalOrders = totalOrders,
                    PendingOrders = pendingOrders,
                    ProcessingOrders = processingOrders,
                    DeliveredOrders = deliveredOrders,
                    CancelledOrders = cancelledOrders,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue
                };

                return ApiResponse<OrderStatsDto>.SuccessResponse(
                    stats,
                    "Estadísticas obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de pedidos");
                return ApiResponse<OrderStatsDto>.ErrorResponse(
                    "Error al obtener estadísticas",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }

        private OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber ?? string.Empty,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                Date = order.CreatedAt,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                ShippingCost = order.Shipping,
                Total = order.Total,
                PaymentMethod = order.PaymentMethod,
                TrackingNumber = order.TrackingNumber,
                Notes = order.Notes,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.ProductImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price,
                    Subtotal = i.Subtotal
                }).ToList(),
                ShippingAddress = order.ShippingAddressDetails != null ? new ShippingAddressDto
                {
                    FullName = order.ShippingAddressDetails.FullName,
                    Phone = order.ShippingAddressDetails.Phone,
                    Street = order.ShippingAddressDetails.Street,
                    City = order.ShippingAddressDetails.City,
                    State = order.ShippingAddressDetails.State,
                    PostalCode = order.ShippingAddressDetails.PostalCode,
                    Country = order.ShippingAddressDetails.Country
                } : null,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }
}
