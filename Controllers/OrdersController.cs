using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
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
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId;
            try
            {
                userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);
            }
            catch
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
                Status = "Completed"
            };

            order.Items = orderItems;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new { orderId = order.Id, message = "Pedido creado con éxito" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);
            
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var result = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Total = o.Total,
                Status = o.Status,
                Items = o.Items.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);
            
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .SingleOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
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
    }
}
