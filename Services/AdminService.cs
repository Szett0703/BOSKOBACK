using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    public interface IAdminService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<OrderDto>> GetRecentOrdersAsync(int limit = 5);
        Task<List<TopProductDto>> GetTopProductsAsync(int limit = 5, string period = "month");
        Task<List<ActivityDto>> GetRecentActivityAsync(int limit = 5);
        Task<UnreadCountDto> GetUnreadNotificationsCountAsync(int userId);
        Task<ChartDataDto> GetSalesChartDataAsync(int months = 6);
        Task<ChartDataDto> GetOrdersStatusChartDataAsync();
        
        Task<PagedResult<OrderDto>> GetOrdersAsync(int page, int limit, string? status, string? search);
        Task<OrderDetailDto?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int id, string status, string? note);
        Task<OrderOperationResult> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<OrderOperationResult> CancelOrderAsync(int id, string reason);
        
        Task<PagedResult<AdminUserDto>> GetUsersAsync(int page, int limit, string? role, string? search);
        Task<bool> UpdateUserRoleAsync(int id, string role);
        Task<bool> ToggleUserStatusAsync(int id);
    }

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(AppDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var lastMonth = now.AddMonths(-1);

            // Sales statistics
            var totalSales = await _context.Orders
                .Where(o => o.Status != "cancelled")
                .SumAsync(o => o.Total);

            var lastMonthSales = await _context.Orders
                .Where(o => o.Status != "cancelled" && o.CreatedAt >= lastMonth)
                .SumAsync(o => o.Total);

            var salesTrend = totalSales > 0 ? (double)(lastMonthSales / totalSales * 100) : 0;

            // Orders statistics
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "pending");
            var processingOrders = await _context.Orders.CountAsync(o => o.Status == "processing");
            var deliveredOrders = await _context.Orders.CountAsync(o => o.Status == "delivered");
            var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "cancelled");

            var lastMonthOrders = await _context.Orders.CountAsync(o => o.CreatedAt >= lastMonth);
            var ordersTrend = totalOrders > 0 ? (double)(lastMonthOrders * 100.0 / totalOrders) : 0;

            // Customers statistics
            var totalCustomers = await _context.Users.CountAsync();
            var activeCustomers = await _context.Users
                .CountAsync(u => u.IsActive && u.Role == "Customer");
            
            var lastMonthCustomers = await _context.Users.CountAsync(u => u.CreatedAt >= lastMonth);
            var customersTrend = totalCustomers > 0 ? (double)(lastMonthCustomers * 100.0 / totalCustomers) : 0;

            // Products statistics
            var totalProducts = await _context.Products.CountAsync();
            var inStockProducts = await _context.Products.CountAsync(p => p.Stock > 0);
            var outOfStockProducts = totalProducts - inStockProducts;

            var lastMonthProducts = await _context.Products.CountAsync(p => p.CreatedAt >= lastMonth);
            var productsTrend = totalProducts > 0 ? (double)(lastMonthProducts * 100.0 / totalProducts) : 0;

            return new DashboardStatsDto
            {
                Sales = new SalesStat
                {
                    Total = totalSales,
                    Trend = Math.Round(salesTrend, 1)
                },
                Orders = new OrdersStat
                {
                    Total = totalOrders,
                    Trend = Math.Round(ordersTrend, 1),
                    Pending = pendingOrders,
                    Processing = processingOrders,
                    Delivered = deliveredOrders,
                    Cancelled = cancelledOrders
                },
                Customers = new CustomersStat
                {
                    Total = totalCustomers,
                    Trend = Math.Round(customersTrend, 1),
                    Active = activeCustomers
                },
                Products = new ProductsStat
                {
                    Total = totalProducts,
                    Trend = Math.Round(productsTrend, 1),
                    InStock = inStockProducts,
                    OutOfStock = outOfStockProducts
                }
            };
        }

        public async Task<List<OrderDto>> GetRecentOrdersAsync(int limit = 5)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerEmail = o.CustomerEmail,
                    Items = o.Items.Count,
                    Amount = o.Total,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(int limit = 5, string period = "month")
        {
            var cutoffDate = period switch
            {
                "week" => DateTime.UtcNow.AddDays(-7),
                "year" => DateTime.UtcNow.AddYears(-1),
                _ => DateTime.UtcNow.AddMonths(-1)
            };

            var topProducts = await _context.OrderItems
                .Where(oi => oi.Order!.CreatedAt >= cutoffDate && oi.Order.Status != "cancelled")
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    Sales = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Subtotal)
                })
                .OrderByDescending(x => x.Sales)
                .Take(limit)
                .ToListAsync();

            var result = new List<TopProductDto>();
            foreach (var item in topProducts)
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                result.Add(new TopProductDto
                {
                    Id = item.ProductId,
                    Name = item.ProductName,
                    Category = product?.Category?.Name ?? "Sin categoría",
                    Sales = item.Sales,
                    Revenue = item.Revenue,
                    ImageUrl = product?.Image
                });
            }

            return result;
        }

        public async Task<List<ActivityDto>> GetRecentActivityAsync(int limit = 5)
        {
            return await _context.ActivityLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    Text = a.Text,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();
        }

        public async Task<UnreadCountDto> GetUnreadNotificationsCountAsync(int userId)
        {
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return new UnreadCountDto { Count = count };
        }

        public async Task<ChartDataDto> GetSalesChartDataAsync(int months = 6)
        {
            var startDate = DateTime.UtcNow.AddMonths(-months);
            
            var salesData = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.Status != "cancelled")
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(o => o.Total)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var labels = salesData.Select(s => $"{GetMonthName(s.Month)} {s.Year}").ToList();
            var data = salesData.Select(s => s.Total).ToList();

            return new ChartDataDto
            {
                Labels = labels,
                Datasets = new List<ChartDatasetDto>
                {
                    new ChartDatasetDto
                    {
                        Label = "Ventas",
                        Data = data
                    }
                }
            };
        }

        public async Task<ChartDataDto> GetOrdersStatusChartDataAsync()
        {
            var pending = await _context.Orders.CountAsync(o => o.Status == "pending");
            var processing = await _context.Orders.CountAsync(o => o.Status == "processing");
            var delivered = await _context.Orders.CountAsync(o => o.Status == "delivered");
            var cancelled = await _context.Orders.CountAsync(o => o.Status == "cancelled");

            return new ChartDataDto
            {
                Labels = new List<string> { "Pendientes", "Procesando", "Entregados", "Cancelados" },
                Datasets = new List<ChartDatasetDto>
                {
                    new ChartDatasetDto
                    {
                        Data = new List<decimal> { pending, processing, delivered, cancelled },
                        BackgroundColor = new List<string> { "#fbbf24", "#3b82f6", "#10b981", "#ef4444" }
                    }
                }
            };
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(int page, int limit, string? status, string? search)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                query = query.Where(o => o.Status == status.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.CustomerName.Contains(search) || 
                                       o.CustomerEmail.Contains(search) ||
                                       o.Id.ToString().Contains(search));
            }

            var total = await query.CountAsync();
            var pages = (int)Math.Ceiling(total / (double)limit);

            var orders = await query
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerEmail = o.CustomerEmail,
                    Items = o.Items.Count,
                    Amount = o.Total,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                Data = orders,
                Total = total,
                Page = page,
                Pages = pages
            };
        }

        public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.ShippingAddressDetails)  // Agregar este Include
                    .Include(o => o.StatusHistory)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return null;

                // Usar ShippingAddressDetails si existe, sino parsear el string legacy
                ShippingAddressInfo shippingAddress;
                
                if (order.ShippingAddressDetails != null)
                {
                    // Nuevo sistema con tabla ShippingAddresses
                    shippingAddress = new ShippingAddressInfo
                    {
                        Street = order.ShippingAddressDetails.Street,
                        City = order.ShippingAddressDetails.City,
                        State = order.ShippingAddressDetails.State,
                        ZipCode = order.ShippingAddressDetails.PostalCode,
                        Country = order.ShippingAddressDetails.Country
                    };
                }
                else
                {
                    // Sistema legacy: parsear el string (compatibilidad con pedidos antiguos)
                    var addressParts = order.ShippingAddress.Split(',').Select(p => p.Trim()).ToArray();
                    shippingAddress = new ShippingAddressInfo
                    {
                        Street = addressParts.Length > 0 ? addressParts[0] : "",
                        City = addressParts.Length > 1 ? addressParts[1] : "",
                        State = addressParts.Length > 2 ? addressParts[2] : "",
                        ZipCode = addressParts.Length > 3 ? addressParts[3] : "",
                        Country = addressParts.Length > 4 ? addressParts[4] : ""
                    };
                }

                return new OrderDetailDto
                {
                    Id = order.Id,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    Items = order.Items.Count,
                    Amount = order.Total,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    Customer = new CustomerInfo
                    {
                        Id = order.CustomerId,
                        Name = order.CustomerName,
                        Email = order.CustomerEmail,
                        Phone = order.Customer?.Phone
                    },
                    ShippingAddress = shippingAddress,
                    OrderItems = order.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        Name = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        Subtotal = i.Subtotal,
                        ImageUrl = i.Product?.Image
                    }).ToList(),
                    Subtotal = order.Subtotal,
                    Shipping = order.Shipping,
                    Total = order.Total,
                    PaymentMethod = order.PaymentMethod,
                    StatusHistory = order.StatusHistory
                        .OrderByDescending(h => h.Timestamp)
                        .Select(h => new StatusHistoryDto
                        {
                            Status = h.Status,
                            Timestamp = h.Timestamp,
                            Note = h.Note
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo detalles del pedido {OrderId}", id);
                throw; // Re-lanzar la excepción para que el controller la maneje
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status, string? note)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            // Validate status transitions
            var validStatuses = new[] { "pending", "processing", "delivered", "cancelled" };
            if (!validStatuses.Contains(status.ToLower()))
                return false;

            order.Status = status.ToLower();
            order.UpdatedAt = DateTime.UtcNow;

            // Agregar al historial
            _context.OrderStatusHistory.Add(new OrderStatusHistory
            {
                OrderId = id,
                Status = status.ToLower(),
                Note = note,
                Timestamp = DateTime.UtcNow
            });

            // Log activity
            _context.ActivityLogs.Add(new ActivityLog
            {
                Type = "order",
                Text = $"Pedido #{id} actualizado a {status}",
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderOperationResult> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.ShippingAddressDetails)
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return new OrderOperationResult
                    {
                        Success = false,
                        Message = "Pedido no encontrado"
                    };
                }

                // Validar que el pedido esté en estado 'pending'
                if (order.Status.ToLower() != "pending")
                {
                    return new OrderOperationResult
                    {
                        Success = false,
                        Message = "No se puede editar un pedido que no está en estado 'pending'"
                    };
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
                if (!string.IsNullOrWhiteSpace(dto.Notes))
                {
                    order.Notes = dto.Notes;
                }

                // Actualizar timestamp
                order.UpdatedAt = DateTime.UtcNow;

                // Log activity
                _context.ActivityLogs.Add(new ActivityLog
                {
                    Type = "order",
                    Text = $"Pedido #{id} editado por administrador",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                // Obtener el pedido actualizado con todos los detalles
                var updatedOrder = await GetOrderByIdAsync(id);

                return new OrderOperationResult
                {
                    Success = true,
                    Message = "Pedido actualizado exitosamente",
                    Data = updatedOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando pedido {OrderId}", id);
                return new OrderOperationResult
                {
                    Success = false,
                    Message = $"Error al actualizar el pedido: {ex.Message}"
                };
            }
        }

        public async Task<OrderOperationResult> CancelOrderAsync(int id, string reason)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.StatusHistory)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return new OrderOperationResult
                    {
                        Success = false,
                        Message = "Pedido no encontrado"
                    };
                }

                // Validar que se puede cancelar
                if (order.Status.ToLower() == "delivered")
                {
                    return new OrderOperationResult
                    {
                        Success = false,
                        Message = "No se puede cancelar un pedido que ya fue entregado"
                    };
                }

                if (order.Status.ToLower() == "cancelled")
                {
                    return new OrderOperationResult
                    {
                        Success = false,
                        Message = "El pedido ya está cancelado"
                    };
                }

                // Restaurar stock de productos
                foreach (var item in order.Items)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                        _logger.LogInformation("Stock restaurado: Producto {ProductId} +{Quantity} unidades", 
                            item.ProductId, item.Quantity);
                    }
                }

                // Actualizar estado a cancelado
                order.Status = "cancelled";
                order.UpdatedAt = DateTime.UtcNow;

                // Registrar en historial
                _context.OrderStatusHistory.Add(new OrderStatusHistory
                {
                    OrderId = id,
                    Status = "cancelled",
                    Note = $"Cancelado por administrador: {reason}",
                    Timestamp = DateTime.UtcNow
                });

                // Log activity
                _context.ActivityLogs.Add(new ActivityLog
                {
                    Type = "order",
                    Text = $"Pedido #{id} cancelado por administrador. Razón: {reason}",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                _logger.LogInformation("Pedido {OrderId} cancelado exitosamente. Razón: {Reason}", id, reason);

                return new OrderOperationResult
                {
                    Success = true,
                    Message = "Pedido cancelado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelando pedido {OrderId}", id);
                return new OrderOperationResult
                {
                    Success = false,
                    Message = $"Error al cancelar el pedido: {ex.Message}"
                };
            }
        }

        public async Task<PagedResult<AdminUserDto>> GetUsersAsync(int page, int limit, string? role, string? search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }

            var total = await query.CountAsync();
            var pages = (int)Math.Ceiling(total / (double)limit);

            var users = await query
                .OrderBy(u => u.Name)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    OrdersCount = _context.Orders.Count(o => o.CustomerId == u.Id)
                })
                .ToListAsync();

            return new PagedResult<AdminUserDto>
            {
                Data = users,
                Total = total,
                Page = page,
                Pages = pages
            };
        }

        public async Task<bool> UpdateUserRoleAsync(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            // Log activity
            _context.ActivityLogs.Add(new ActivityLog
            {
                Type = "user",
                Text = $"Usuario {user.Name} cambió de rol a {role}",
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Log activity
            _context.ActivityLogs.Add(new ActivityLog
            {
                Type = "user",
                Text = $"Usuario {user.Name} {(user.IsActive ? "activado" : "desactivado")}",
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        private string GetMonthName(int month)
        {
            var months = new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                               "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            return months[month - 1];
        }
    }
}
