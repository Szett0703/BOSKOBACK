using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BOSKOBACK.Data;
using BOSKOBACK.DTOs;
using BOSKOBACK.Models;

namespace BOSKOBACK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly BoskoDbContext _context;

        public OrdersController(BoskoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<CreateOrderItemDto> itemsDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            if (itemsDto == null || itemsDto.Count == 0)
            {
                return BadRequest(new { message = "No hay items en el pedido." });
            }

            var productIds = itemsDto.Select(i => i.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            
            if (products.Count != itemsDto.Count)
            {
                return BadRequest(new { message = "Alguno de los productos no existe." });
            }

            decimal subtotal = 0;
            var orderItems = new List<OrderItem>();
            
            foreach (var itemDto in itemsDto)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);
                subtotal += product.Price * itemDto.Quantity;
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });
            }

            decimal tax = subtotal * 0.10m;
            decimal total = subtotal + tax;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Total = total,
                Status = "Pending"
            };

            order.Items = orderItems;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new { orderId = order.Id, message = "Pedido creado con éxito" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] string? status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<Order> query = _context.Orders;

            if (userRole == "Customer")
            {
                query = query.Where(o => o.UserId == userId);
            }
            else if (userRole == "Admin" || userRole == "Employee")
            {
                // Admin and Employee can see all orders
                query = query.Include(o => o.User);
            }
            else
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var result = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Total = o.Total,
                Status = o.Status,
                Items = new List<OrderItemDto>() // Don't include items in list view for performance
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Authorization check
            if (userRole == "Customer" && order.UserId != userId)
            {
                return Forbid();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Total = order.Total,
                Status = order.Status,
                Items = order.Items.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = dto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
