using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    // ============================================
    // IMPLEMENTACIÓN: SERVICIO DE USUARIOS (ADMIN)
    // ============================================
    public class UserAdminService : IUserAdminService
    {
        private readonly AppDbContext _context;
        private readonly IActivityLogService _activityLog;
        private readonly ILogger<UserAdminService> _logger;

        public UserAdminService(AppDbContext context, IActivityLogService activityLog, ILogger<UserAdminService> logger)
        {
            _context = context;
            _activityLog = activityLog;
            _logger = logger;
        }

        public async Task<ApiResponse<UserAdminResponseDto>> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<UserAdminResponseDto>.ErrorResponse("Usuario no encontrado");
                }

                // Validar que el email sea único
                var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != id);
                if (emailExists)
                {
                    return ApiResponse<UserAdminResponseDto>.ErrorResponse("El email ya está registrado");
                }

                // Validar rol
                var validRoles = new[] { "Admin", "Employee", "Customer" };
                if (!validRoles.Contains(dto.Role))
                {
                    return ApiResponse<UserAdminResponseDto>.ErrorResponse("Rol inválido");
                }

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.Phone = dto.Phone;
                user.Role = dto.Role;
                user.IsActive = dto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("user", $"Usuario actualizado: {user.Name}");

                var result = await GetUserByIdAsync(user.Id);
                return ApiResponse<UserAdminResponseDto>.SuccessResponse(result.Data!, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
                return ApiResponse<UserAdminResponseDto>.ErrorResponse("Error al actualizar el usuario");
            }
        }

        public async Task<ApiResponse<bool>> ToggleUserStatusAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var action = user.IsActive ? "activado" : "desactivado";
                await _activityLog.LogActivityAsync("user", $"Usuario {action}: {user.Name}");

                return ApiResponse<bool>.SuccessResponse(true, $"Usuario {action} exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de usuario {UserId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al cambiar el estado del usuario");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                // Verificar si es el último admin
                if (user.Role == "Admin")
                {
                    var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin" && u.IsActive);
                    if (adminCount <= 1)
                    {
                        return ApiResponse<bool>.ErrorResponse("No se puede eliminar el último administrador activo");
                    }
                }

                var userName = user.Name;
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await _activityLog.LogActivityAsync("user", $"Usuario eliminado: {userName}");

                return ApiResponse<bool>.SuccessResponse(true, "Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar el usuario");
            }
        }

        public async Task<ApiResponse<UserAdminResponseDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<UserAdminResponseDto>.ErrorResponse("Usuario no encontrado");
                }

                // Obtener estadísticas del usuario
                var orders = await _context.Orders.Where(o => o.CustomerId == id).ToListAsync();

                var dto = new UserAdminResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Provider = user.Provider,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    TotalOrders = orders.Count,
                    TotalSpent = orders.Sum(o => o.Total)
                };

                return ApiResponse<UserAdminResponseDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
                return ApiResponse<UserAdminResponseDto>.ErrorResponse("Error al obtener el usuario");
            }
        }

        public async Task<ApiResponse<PagedResponse<UserListDto>>> GetUsersAsync(UserFilterDto filter)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Filtros
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    query = query.Where(u => u.Name.Contains(filter.Search) || u.Email.Contains(filter.Search));
                }

                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    query = query.Where(u => u.Role == filter.Role);
                }

                if (filter.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);
                }

                // Ordenamiento
                query = filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                    "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                    _ => filter.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var users = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<UserListDto>
                {
                    Items = users,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };

                return ApiResponse<PagedResponse<UserListDto>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de usuarios");
                return ApiResponse<PagedResponse<UserListDto>>.ErrorResponse("Error al obtener la lista de usuarios");
            }
        }

        public async Task<ApiResponse<bool>> ChangeUserRoleAsync(int id, string role)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                var validRoles = new[] { "Admin", "Employee", "Customer" };
                if (!validRoles.Contains(role))
                {
                    return ApiResponse<bool>.ErrorResponse("Rol inválido");
                }

                // Si está quitando el rol Admin, verificar que no sea el último
                if (user.Role == "Admin" && role != "Admin")
                {
                    var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin" && u.IsActive);
                    if (adminCount <= 1)
                    {
                        return ApiResponse<bool>.ErrorResponse("No se puede cambiar el rol del último administrador activo");
                    }
                }

                user.Role = role;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _activityLog.LogActivityAsync("user", $"Rol de usuario cambiado: {user.Name} → {role}");

                return ApiResponse<bool>.SuccessResponse(true, "Rol actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar rol de usuario {UserId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al cambiar el rol del usuario");
            }
        }
    }
}
