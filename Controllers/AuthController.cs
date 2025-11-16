using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using DBTest_BACK.Data;
using DBTest_BACK.Models;
using DBTest_BACK.DTOs;
using DBTest_BACK.Services;

namespace DBTest_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext context,
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Buscar usuario por email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (user == null)
                {
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Verificar si el usuario está activo
                if (!user.IsActive)
                {
                    return Unauthorized(new { message = "Usuario inactivo. Contacte al administrador" });
                }

                // Verificar si el usuario es de Google
                if (user.Provider == "Google")
                {
                    return BadRequest(new { message = "Este usuario debe iniciar sesión con Google" });
                }

                // Verificar contraseña
                if (string.IsNullOrEmpty(user.PasswordHash) || 
                    !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Generar JWT
                var token = _authService.GenerateJwtToken(user);

                // Preparar respuesta
                var response = new AuthResponseDto
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                        Provider = user.Provider,
                        Phone = user.Phone,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    }
                };

                _logger.LogInformation($"Login exitoso para usuario: {user.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar si el email ya existe
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (existingUser != null)
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }

                // Hashear contraseña con BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Crear nuevo usuario
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email.ToLower(),
                    PasswordHash = passwordHash,
                    Phone = dto.Phone,
                    Role = "Customer", // Siempre Customer en registro público
                    Provider = "Local",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generar JWT
                var token = _authService.GenerateJwtToken(user);

                // Preparar respuesta
                var response = new AuthResponseDto
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                        Provider = user.Provider,
                        Phone = user.Phone,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    }
                };

                _logger.LogInformation($"Usuario registrado exitosamente: {user.Email}");
                return CreatedAtAction(nameof(Login), new { email = user.Email }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/auth/google-login
        [HttpPost("google-login")]
        public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validar token de Google
                Google.Apis.Auth.GoogleJsonWebSignature.Payload payload;
                
                try
                {
                    var validationSettings = new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { "YOUR_GOOGLE_CLIENT_ID" } // TODO: Configurar desde appsettings
                    };
                    
                    payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(dto.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Token de Google inválido: {ex.Message}");
                    return Unauthorized(new { message = "Token de Google inválido" });
                }

                // Buscar usuario por email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == payload.Email.ToLower());

                if (user == null)
                {
                    // Crear nuevo usuario de Google
                    user = new User
                    {
                        Name = payload.Name,
                        Email = payload.Email.ToLower(),
                        PasswordHash = null, // Sin password para usuarios de Google
                        Role = "Customer",
                        Provider = "Google",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Nuevo usuario de Google creado: {user.Email}");
                }
                else
                {
                    // Verificar si el usuario está activo
                    if (!user.IsActive)
                    {
                        return Unauthorized(new { message = "Usuario inactivo. Contacte al administrador" });
                    }

                    // Actualizar a proveedor Google si era local
                    if (user.Provider != "Google")
                    {
                        user.Provider = "Google";
                        user.PasswordHash = null;
                        user.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }

                // Generar JWT
                var token = _authService.GenerateJwtToken(user);

                // Preparar respuesta
                var response = new AuthResponseDto
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                        Provider = user.Provider,
                        Phone = user.Phone,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    }
                };

                _logger.LogInformation($"Login con Google exitoso para: {user.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Google login");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<ActionResult<MessageResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                // Por seguridad, siempre retornar el mismo mensaje
                var message = "Si el email existe, recibirás instrucciones para restablecer tu contraseña";

                if (user == null || user.Provider == "Google")
                {
                    return Ok(new MessageResponseDto { Message = message });
                }

                // Generar token de reset
                var resetToken = _authService.GenerateResetToken();
                var expiresAt = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

                var passwordResetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = resetToken,
                    ExpiresAt = expiresAt,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PasswordResetTokens.Add(passwordResetToken);
                await _context.SaveChangesAsync();

                // TODO: Enviar email con el token
                // Por ahora solo logueamos el token (en producción NUNCA hacer esto)
                _logger.LogInformation($"Token de reset generado para {user.Email}: {resetToken}");
                
                // En desarrollo, podemos retornar el token para testing
                #if DEBUG
                return Ok(new { 
                    message = message,
                    token = resetToken, // Solo para desarrollo
                    expiresAt = expiresAt
                });
                #else
                return Ok(new MessageResponseDto { Message = message });
                #endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en forgot-password");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<ActionResult<MessageResponseDto>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (user == null)
                {
                    return BadRequest(new { message = "Token inválido o expirado" });
                }

                // Buscar token válido
                var resetToken = await _context.PasswordResetTokens
                    .Where(t => t.UserId == user.Id && 
                               t.Token == dto.Token && 
                               !t.IsUsed && 
                               t.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (resetToken == null)
                {
                    return BadRequest(new { message = "Token inválido o expirado" });
                }

                // Hashear nueva contraseña
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                // Marcar token como usado
                resetToken.IsUsed = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Contraseña restablecida para usuario: {user.Email}");
                
                return Ok(new MessageResponseDto { Message = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en reset-password");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/auth/init-users (Solo para desarrollo - inicializar passwords)
        [HttpPost("init-users")]
        public async Task<IActionResult> InitializeUsers()
        {
            try
            {
                var password = "Bosko123!";
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var usersToUpdate = await _context.Users
                    .Where(u => u.Provider == "Local" && 
                               (u.PasswordHash == null || u.PasswordHash == ""))
                    .ToListAsync();

                foreach (var user in usersToUpdate)
                {
                    user.PasswordHash = passwordHash;
                    user.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Passwords inicializados para {usersToUpdate.Count} usuarios",
                    password = password,
                    users = usersToUpdate.Select(u => new { u.Email, u.Role })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inicializando usuarios");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
