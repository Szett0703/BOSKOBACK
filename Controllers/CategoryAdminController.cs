using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    /// <summary>
    /// Controlador para gestión de categorías (Admin/Employee)
    /// </summary>
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Admin,Employee")]
    public class CategoryAdminController : ControllerBase
    {
        private readonly ICategoryAdminService _categoryService;
        private readonly ILogger<CategoryAdminController> _logger;

        public CategoryAdminController(ICategoryAdminService categoryService, ILogger<CategoryAdminController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las categorías con contador de productos
        /// </summary>
        /// <returns>Lista de categorías</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetCategories()
        {
            var result = await _categoryService.GetCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtener categorías simples (solo ID y nombre para dropdowns)
        /// </summary>
        /// <returns>Lista simple de categorías</returns>
        [HttpGet("simple")]
        [ProducesResponseType(typeof(ApiResponse<List<SimpleCategoryDto>>), 200)]
        public async Task<ActionResult<ApiResponse<List<SimpleCategoryDto>>>> GetSimpleCategories()
        {
            var result = await _categoryService.GetSimpleCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Crear una nueva categoría
        /// </summary>
        /// <param name="dto">Datos de la categoría</param>
        /// <returns>Categoría creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<CategoryResponseDto>.ErrorResponse(
                    "Error de validación",
                    string.Join(", ", errors)
                ));
            }

            var result = await _categoryService.CreateCategoryAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetCategories), new { }, result);
        }

        /// <summary>
        /// Actualizar una categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Categoría actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> UpdateCategory(int id, [FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<CategoryResponseDto>.ErrorResponse(
                    "Error de validación",
                    string.Join(", ", errors)
                ));
            }

            var result = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("no encontrada"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Eliminar una categoría
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
