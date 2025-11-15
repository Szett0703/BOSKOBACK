using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BOSKOBACK.Data;
using BOSKOBACK.DTOs;
using BOSKOBACK.Models;
using Google.Apis.Auth;

namespace BOSKOBACK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BoskoDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(BoskoDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado." });
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Provider = "Local",
                RoleId = 3 // Customer by default
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load role for token generation
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new { id = user.Id, name = user.Name, email = user.Email, role = user.Role.Name }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            if (user.Provider != "Local")
            {
                return BadRequest(new { message = "Este usuario se registró con Google. Usa 'Iniciar con Google'." });
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            bool validPass = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!validPass)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new { id = user.Id, name = user.Name, email = user.Email, role = user.Role.Name }
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings());
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Token de Google inválido." });
            }

            string googleEmail = payload.Email;
            string name = payload.Name ?? payload.Email;

            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == googleEmail);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = googleEmail,
                    Provider = "Google",
                    PasswordHash = null,
                    RoleId = 3 // Customer by default
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new { id = user.Id, name = user.Name, email = user.Email, role = user.Role.Name }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Ok(new { message = "Si existe una cuenta con ese email, se ha enviado un enlace de recuperación." });
            }

            string resetToken = Guid.NewGuid().ToString();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            string resetLink = $"http://localhost:4200/reset-password?token={resetToken}&email={user.Email}";
            Console.WriteLine($"[ResetPassword] Enlace de reset para {user.Email}: {resetLink}");

            return Ok(new { message = "Si existe una cuenta con ese email, se ha enviado un enlace de recuperación." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Solicitud inválida." });
            }

            if (user.ResetToken == null || user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Token de recuperación inválido o expirado." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contraseña restablecida exitosamente." });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(new { message = "No se puede cambiar la contraseña." });
            }

            bool validPass = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!validPass)
            {
                return BadRequest(new { message = "Contraseña actual incorrecta." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contraseña actualizada exitosamente." });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
