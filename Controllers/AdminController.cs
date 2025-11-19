using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // ==================== DASHBOARD ====================

        /// <summary>
        /// Obtiene las estadísticas del dashboard.
        /// </summary>
        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos para el gráfico de ventas.
        /// </summary>
        [HttpGet("dashboard/sales-chart")]
        public async Task<ActionResult<ChartDataDto>> GetSalesChart([FromQuery] int months = 6)
        {
            try
            {
                var chartData = await _adminService.GetSalesChartDataAsync(months);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales chart data");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos para el gráfico de estado de pedidos.
        /// </summary>
        [HttpGet("dashboard/orders-status")]
        public async Task<ActionResult<ChartDataDto>> GetOrdersStatus()
        {
            try
            {
                var chartData = await _adminService.GetOrdersStatusChartDataAsync();
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders status chart data");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ==================== RECENT DATA ====================

        /// <summary>
        /// Obtiene los pedidos más recientes.
        /// </summary>
        [HttpGet("orders/recent")]
        public async Task<ActionResult<List<OrderDto>>> GetRecentOrders([FromQuery] int limit = 5)
        {
            try
            {
                var orders = await _adminService.GetRecentOrdersAsync(limit);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent orders");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los productos más vendidos.
        /// </summary>
        [HttpGet("products/top-sellers")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopProducts(
            [FromQuery] int limit = 5,
            [FromQuery] string period = "month")
        {
            try
            {
                var products = await _adminService.GetTopProductsAsync(limit, period);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top products");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene la actividad reciente del sistema.
        /// </summary>
        [HttpGet("activity/recent")]
        public async Task<ActionResult<List<ActivityDto>>> GetRecentActivity([FromQuery] int limit = 5)
        {
            try
            {
                var activities = await _adminService.GetRecentActivityAsync(limit);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activity");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ==================== NOTIFICATIONS ====================

        /// <summary>
        /// Obtiene el conteo de notificaciones no leídas.
        /// </summary>
        [HttpGet("notifications/unread-count")]
        public async Task<ActionResult<UnreadCountDto>> GetUnreadNotificationsCount()
        {
            try
            {
                // Obtener userId del token JWT
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var count = await _adminService.GetUnreadNotificationsCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications count");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ==================== ORDERS MANAGEMENT ====================

        /// <summary>
        /// Obtiene lista paginada de pedidos con filtros.
        /// </summary>
        [HttpGet("orders")]
        public async Task<ActionResult> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "createdAt",
            [FromQuery] string? sortOrder = "desc")
        {
            try
            {
                // Validate limit
                if (limit > 100) limit = 100;
                if (limit < 1) limit = 10;
                if (page < 1) page = 1;

                var result = await _adminService.GetOrdersAsync(page, limit, status, search);
            
                // Return response in the format expected by frontend
                return Ok(new
                {
                    orders = result.Data,
                    pagination = new
                    {
                        total = result.Total,
                        page = result.Page,
                        pages = result.Pages,
                        limit = limit
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene detalles completos de un pedido.
        /// </summary>
        [HttpGet("orders/{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _adminService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new 
                    { 
                        error = "Pedido no encontrado",
                        orderId = id
                    });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza el estado de un pedido.
        /// </summary>
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var validStatuses = new[] { "pending", "processing", "delivered", "cancelled" };
                var statusLower = dto.Status.ToLower();
            
                if (!validStatuses.Contains(statusLower))
                {
                    return BadRequest(new 
                    { 
                        error = "Estado inválido",
                        validStatuses = validStatuses
                    });
                }

                // Validate note length
                if (!string.IsNullOrEmpty(dto.Note) && dto.Note.Length > 500)
                {
                    return BadRequest(new { error = "La nota no puede exceder 500 caracteres" });
                }

                var success = await _adminService.UpdateOrderStatusAsync(id, statusLower, dto.Note);
                if (!success)
                {
                    return NotFound(new 
                    { 
                        error = "Pedido no encontrado",
                        orderId = id
                    });
                }

                return Ok(new 
                { 
                    id = id, 
                    status = statusLower,
                    updatedAt = DateTime.UtcNow,
                    message = "Estado del pedido actualizado exitosamente" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza la dirección de envío y notas de un pedido (solo estado 'pending').
        /// </summary>
        [HttpPut("orders/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new 
                    { 
                        error = "Datos de validación incorrectos",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var result = await _adminService.UpdateOrderAsync(id, dto);
                
                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrado"))
                    {
                        return NotFound(new { error = result.Message });
                    }
                    return BadRequest(new { error = result.Message });
                }

                return Ok(new
                {
                    success = true,
                    message = "Pedido actualizado exitosamente",
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancela un pedido desde el panel de administración (solo estados 'pending' o 'processing').
        /// </summary>
        [HttpPost("orders/{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new 
                    { 
                        error = "La razón de cancelación es obligatoria",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Reason))
                {
                    return BadRequest(new { error = "Debes proporcionar una razón para cancelar el pedido" });
                }

                if (dto.Reason.Length < 10)
                {
                    return BadRequest(new { error = "La razón debe tener al menos 10 caracteres" });
                }

                var result = await _adminService.CancelOrderAsync(id, dto.Reason);
                
                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrado"))
                    {
                        return NotFound(new { error = result.Message });
                    }
                    return BadRequest(new { error = result.Message });
                }

                return Ok(new
                {
                    success = true,
                    message = "Pedido cancelado exitosamente",
                    data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // ==================== USER MANAGEMENT ====================
        // NOTA: Los endpoints de gestión de usuarios se encuentran en AdminUsersController.cs
        // para evitar conflictos de rutas y mejorar la organización del código.
        // Endpoints disponibles en AdminUsersController:
        // - GET    /api/admin/users
        // - GET    /api/admin/users/{id}
        // - PUT    /api/admin/users/{id}
        // - DELETE /api/admin/users/{id}
    }
}
