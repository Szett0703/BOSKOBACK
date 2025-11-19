        using DBTest_BACK.DTOs;
        using DBTest_BACK.Services;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;

        namespace DBTest_BACK.Controllers
        {
            /// <summary>
            /// Controlador para gestión de usuarios (Admin)
            /// </summary>
            [ApiController]
            [Route("api/admin/users")]
            [Authorize(Roles = "Admin")]
            public class AdminUsersController : ControllerBase
            {
                private readonly IUserAdminService _userService;
                private readonly ILogger<AdminUsersController> _logger;

                public AdminUsersController(IUserAdminService userService, ILogger<AdminUsersController> logger)
                {
                    _userService = userService;
                    _logger = logger;
                }

                /// <summary>
                /// Obtener lista de usuarios con filtros y paginación
                /// </summary>
                /// <param name="filter">Filtros de búsqueda</param>
                /// <returns>Lista paginada de usuarios</returns>
                /// <response code="200">Lista de usuarios</response>
                [HttpGet]
                [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserListDto>>), 200)]
                public async Task<ActionResult<ApiResponse<PagedResponse<UserListDto>>>> GetUsers([FromQuery] UserFilterDto filter)
                {
                    var result = await _userService.GetUsersAsync(filter);
                    return Ok(result);
                }

                /// <summary>
                /// Obtener un usuario por ID con estadísticas
                /// </summary>
                /// <param name="id">ID del usuario</param>
                /// <returns>Detalles del usuario</returns>
                /// <response code="200">Usuario encontrado</response>
                /// <response code="404">Usuario no encontrado</response>
                [HttpGet("{id}")]
                [ProducesResponseType(typeof(ApiResponse<UserAdminResponseDto>), 200)]
                [ProducesResponseType(404)]
                public async Task<ActionResult<ApiResponse<UserAdminResponseDto>>> GetUser(int id)
                {
                    var result = await _userService.GetUserByIdAsync(id);

                    if (!result.Success)
                    {
                        return NotFound(result);
                    }

                    return Ok(result);
                }

                /// <summary>
                /// Actualizar información de un usuario
                /// </summary>
                /// <param name="id">ID del usuario</param>
                /// <param name="dto">Datos actualizados</param>
                /// <returns>Usuario actualizado</returns>
                /// <response code="200">Usuario actualizado exitosamente</response>
                /// <response code="400">Datos inválidos o email duplicado</response>
                /// <response code="404">Usuario no encontrado</response>
                [HttpPut("{id}")]
                [ProducesResponseType(typeof(ApiResponse<UserAdminResponseDto>), 200)]
                [ProducesResponseType(400)]
                [ProducesResponseType(404)]
                public async Task<ActionResult<ApiResponse<UserAdminResponseDto>>> UpdateUser(int id, [FromBody] UserUpdateDto dto)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ApiResponse<UserAdminResponseDto>.ErrorResponse("Datos inválidos",
                            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                    }

                    var result = await _userService.UpdateUserAsync(id, dto);

                    if (!result.Success)
                    {
                        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
                    }

                    return Ok(result);
                }

                /// <summary>
                /// Cambiar el rol de un usuario
                /// </summary>
                /// <param name="id">ID del usuario</param>
                /// <param name="request">Nuevo rol</param>
                /// <returns>Confirmación del cambio</returns>
                /// <response code="200">Rol actualizado exitosamente</response>
                /// <response code="400">Rol inválido</response>
                /// <response code="404">Usuario no encontrado</response>
                [HttpPatch("{id}/role")]
                [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
                [ProducesResponseType(400)]
                [ProducesResponseType(404)]
                public async Task<ActionResult<ApiResponse<bool>>> ChangeUserRole(int id, [FromBody] ChangeRoleRequest request)
                {
                    if (string.IsNullOrWhiteSpace(request.Role))
                    {
                        return BadRequest(ApiResponse<bool>.ErrorResponse("El rol es requerido"));
                    }

                    var result = await _userService.ChangeUserRoleAsync(id, request.Role);

                    if (!result.Success)
                    {
                        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
                    }

                    return Ok(result);
                }

                /// <summary>
                /// Activar o desactivar un usuario
                /// </summary>
                /// <param name="id">ID del usuario</param>
                /// <returns>Confirmación del cambio</returns>
                /// <response code="200">Estado cambiado exitosamente</response>
                /// <response code="404">Usuario no encontrado</response>
                [HttpPatch("{id}/toggle-status")]
                [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
                [ProducesResponseType(404)]
                public async Task<ActionResult<ApiResponse<bool>>> ToggleUserStatus(int id)
                {
                    var result = await _userService.ToggleUserStatusAsync(id);

                    if (!result.Success)
                    {
                        return NotFound(result);
                    }

                    return Ok(result);
                }

                /// <summary>
                /// Eliminar un usuario permanentemente
                /// </summary>
                /// <param name="id">ID del usuario</param>
                /// <returns>Confirmación de eliminación</returns>
                /// <response code="200">Usuario eliminado exitosamente</response>
                /// <response code="400">No se puede eliminar (último admin)</response>
                /// <response code="404">Usuario no encontrado</response>
                [HttpDelete("{id}")]
                [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
                [ProducesResponseType(400)]
                [ProducesResponseType(404)]
                public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
                {
                    var result = await _userService.DeleteUserAsync(id);

                    if (!result.Success)
                    {
                        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
                    }

                    return Ok(result);
                }
            }

            /// <summary>
            /// DTO para cambio de rol
            /// </summary>
            public class ChangeRoleRequest
            {
                public string Role { get; set; } = string.Empty;
            }
        }
