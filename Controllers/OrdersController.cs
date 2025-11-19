using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    /// <summary>
    /// Controlador para gestión de pedidos
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Crear un nuevo pedido
        /// </summary>
        /// <param name="dto">Datos del pedido</param>
        /// <returns>Pedido creado</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error de validación",
                    errors
                ));
            }

            var result = await _orderService.CreateOrderAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Obtener un pedido por ID
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <returns>Detalles del pedido</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener todos los pedidos con filtros y paginación
        /// </summary>
        /// <param name="filters">Filtros de búsqueda</param>
        /// <returns>Lista paginada de pedidos</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderListDto>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderListDto>>>> GetOrders([FromQuery] OrderFilterDto filters)
        {
            var result = await _orderService.GetOrdersAsync(filters);
            return Ok(result);
        }

        /// <summary>
        /// Obtener pedidos de un cliente específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de pedidos del cliente</returns>
        [HttpGet("customer/{customerId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<OrderListDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<List<OrderListDto>>>> GetCustomerOrders(int customerId)
        {
            // Verificar que el usuario solo pueda ver sus propios pedidos (a menos que sea admin)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Solo admin/employee pueden ver pedidos de otros clientes
            if (userId != customerId && userRole != "Admin" && userRole != "Employee")
            {
                return Forbid();
            }

            var result = await _orderService.GetCustomerOrdersAsync(customerId);
            return Ok(result);
        }

        /// <summary>
        /// Obtener mis propios pedidos (del usuario autenticado)
        /// </summary>
        /// <returns>Lista de mis pedidos</returns>
        [HttpGet("my-orders")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<OrderListDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<List<OrderListDto>>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var result = await _orderService.GetCustomerOrdersAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Actualizar el estado de un pedido
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="dto">Nuevos datos del estado</param>
        /// <returns>Pedido actualizado</returns>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrderStatus(
            int id,
            [FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error de validación",
                    errors
                ));
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Actualizar datos de un pedido (dirección y notas) - Solo si está pendiente
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="dto">Nuevos datos del pedido</param>
        /// <returns>Pedido actualizado</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrder(
            int id,
            [FromBody] OrderUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<OrderResponseDto>.ErrorResponse(
                    "Error de validación",
                    errors
                ));
            }

            // Verificar que el usuario solo pueda actualizar sus propios pedidos (a menos que sea admin)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Obtener el pedido para verificar ownership
            var orderResult = await _orderService.GetOrderByIdAsync(id);
            if (!orderResult.Success || orderResult.Data == null)
            {
                return NotFound(orderResult);
            }

            // Solo admin o el dueño del pedido pueden actualizarlo
            if (orderResult.Data.CustomerId != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var result = await _orderService.UpdateOrderAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cancelar un pedido
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="request">Razón de cancelación</param>
        /// <returns>Confirmación de cancelación</returns>
        [HttpPost("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(int id, [FromBody] CancelOrderRequest request)
        {
            // Verificar que el usuario solo pueda cancelar sus propios pedidos (a menos que sea admin)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Obtener el pedido para verificar ownership
            var orderResult = await _orderService.GetOrderByIdAsync(id);
            if (!orderResult.Success || orderResult.Data == null)
            {
                return NotFound(orderResult);
            }

            // Solo admin/employee o el dueño del pedido pueden cancelarlo
            if (orderResult.Data.CustomerId != userId && userRole != "Admin" && userRole != "Employee")
            {
                return Forbid();
            }

            var result = await _orderService.CancelOrderAsync(id, request.Reason ?? "Cancelado por el usuario");

            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de pedidos
        /// </summary>
        /// <returns>Estadísticas generales</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(typeof(ApiResponse<OrderStatsDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<OrderStatsDto>>> GetOrderStats()
        {
            var result = await _orderService.GetOrderStatsAsync();
            return Ok(result);
        }
    }

    /// <summary>
    /// Request para cancelar un pedido
    /// </summary>
    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
    }
}
