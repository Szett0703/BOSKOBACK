using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOSKOBACK.Data;
using BOSKOBACK.DTOs;
using BOSKOBACK.Models;

namespace BOSKOBACK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly BoskoDbContext _context;

        public UsersController(BoskoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            var result = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Provider = u.Provider,
                RoleName = u.Role.Name
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado." });
            }

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == dto.RoleName);
            if (role == null)
            {
                return BadRequest(new { message = "Rol inválido." });
            }

            string password = dto.Password ?? Guid.NewGuid().ToString().Substring(0, 8);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Provider = "Local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = role.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Provider = user.Provider,
                RoleName = role.Name
            };

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                user.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var duplicateEmail = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email && u.Id != id);
                if (duplicateEmail)
                {
                    return BadRequest(new { message = "El email ya está en uso." });
                }
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.RoleName))
            {
                var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == dto.RoleName);
                if (role == null)
                {
                    return BadRequest(new { message = "Rol inválido." });
                }
                user.RoleId = role.Id;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent deletion of users with orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
            if (hasOrders)
            {
                return BadRequest(new { message = "No se puede eliminar usuario con historial de pedidos." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
