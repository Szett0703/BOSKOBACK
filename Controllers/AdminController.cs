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

        // ==================== USERS MANAGEMENT (Solo Admin) ====================

        /// <summary>
        /// Obtiene lista paginada de usuarios con filtros.
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<AdminUserDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string? role = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var users = await _adminService.GetUsersAsync(page, limit, role, search);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cambia el rol de un usuario.
        /// </summary>
        [HttpPut("users/{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var validRoles = new[] { "Admin", "Employee", "Customer" };
                if (!validRoles.Contains(dto.Role))
                {
                    return BadRequest(new { message = "Rol inválido" });
                }

                var success = await _adminService.UpdateUserRoleAsync(id, dto.Role);
                if (!success)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new 
                { 
                    id = id, 
                    role = dto.Role,
                    message = "Rol actualizado exitosamente" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Activa o desactiva un usuario.
        /// </summary>
        [HttpPut("users/{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            try
            {
                var success = await _adminService.ToggleUserStatusAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new 
                { 
                    id = id,
                    message = "Estado del usuario actualizado exitosamente" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
