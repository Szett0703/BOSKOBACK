using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using System.Security.Claims;

namespace DBTest_BACK.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IWebHostEnvironment _env;

        public UsersController(
            AppDbContext context, 
            ILogger<UsersController> logger,
            IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var user = await _context.Users
                    .Include(u => u.Preferences)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                var totalOrders = await _context.Orders.CountAsync(o => o.CustomerId == userId);
                var totalSpent = await _context.Orders
                    .Where(o => o.CustomerId == userId)
                    .SumAsync(o => (decimal?)o.Total) ?? 0;

                var profileDto = new UserProfileDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Provider = user.Provider,
                    IsActive = user.IsActive,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    TotalOrders = totalOrders,
                    TotalSpent = totalSpent,
                    Preferences = user.Preferences != null ? new UserPreferencesDto
                    {
                        Notifications = user.Preferences.Notifications,
                        Newsletter = user.Preferences.Newsletter,
                        Language = user.Preferences.Language
                    } : null
                };

                return Ok(new { success = true, message = "Perfil obtenido correctamente", data = profileDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState });

            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null || !user.IsActive)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                    return BadRequest(new { success = false, message = "El correo electrónico ya está registrado" });

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.Phone = dto.Phone;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = "Perfil actualizado correctamente", 
                    data = new 
                    {
                        id = user.Id,
                        name = user.Name,
                        email = user.Email,
                        phone = user.Phone,
                        role = user.Role,
                        provider = user.Provider,
                        isActive = user.IsActive,
                        avatarUrl = user.AvatarUrl,
                        createdAt = user.CreatedAt,
                        updatedAt = user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState });

            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null || !user.IsActive)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                if (user.Provider == "Google")
                    return Forbid("Los usuarios de Google no pueden cambiar la contraseña");

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    return BadRequest(new { success = false, message = "La contraseña actual es incorrecta" });

                if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
                    return BadRequest(new { success = false, message = "La nueva contraseña debe ser diferente a la actual" });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Contraseña cambiada correctamente", data = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPut("me/preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var preferences = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

                if (preferences == null)
                {
                    preferences = new UserPreferences
                    {
                        UserId = userId,
                        Notifications = dto.Notifications,
                        Newsletter = dto.Newsletter,
                        Language = dto.Language ?? "es",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserPreferences.Add(preferences);
                }
                else
                {
                    preferences.Notifications = dto.Notifications;
                    preferences.Newsletter = dto.Newsletter;
                    if (!string.IsNullOrEmpty(dto.Language))
                        preferences.Language = dto.Language;
                    preferences.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Preferencias actualizadas correctamente", data = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preferences");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("me/avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return BadRequest(new { success = false, message = "No se proporcionó ningún archivo" });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(avatar.ContentType.ToLower()))
                return BadRequest(new { success = false, message = "Tipo de archivo no permitido. Solo JPEG, PNG o WEBP" });

            if (avatar.Length > 5 * 1024 * 1024)
                return BadRequest(new { success = false, message = "El archivo es demasiado grande. Máximo 5 MB" });

            try
            {
                var userId = GetCurrentUserId();
                var uploadsDir = Path.Combine(_env.WebRootPath ?? "", "uploads", "avatars");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"user-{userId}-{DateTime.UtcNow:yyyyMMdd-HHmmss}{Path.GetExtension(avatar.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.AvatarUrl))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath ?? "", user.AvatarUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    user.AvatarUrl = $"/uploads/avatars/{fileName}";
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                var avatarUrl = $"{Request.Scheme}://{Request.Host}/uploads/avatars/{fileName}";
                return Ok(new { success = true, message = "Avatar actualizado correctamente", data = avatarUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cuenta desactivada correctamente. Puedes reactivarla contactando soporte", data = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user account");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("nameid")?.Value
                           ?? User.FindFirst("sub")?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}
