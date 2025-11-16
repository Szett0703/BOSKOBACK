using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    /// <summary>
    /// Controlador para gestión de categorías (Admin)
    /// </summary>
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Admin,Employee")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminCategoriesController> _logger;

        public AdminCategoriesController(ICategoryService categoryService, ILogger<AdminCategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Crear una nueva categoría
        /// </summary>
        /// <param name="dto">Datos de la categoría</param>
        /// <returns>Categoría creada</returns>
        /// <response code="200">Categoría creada exitosamente</response>
        /// <response code="400">Datos inválidos o categoría duplicada</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<CategoryResponseDto>.ErrorResponse("Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _categoryService.CreateCategoryAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Actualizar una categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Categoría actualizada</returns>
        /// <response code="200">Categoría actualizada exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<CategoryResponseDto>.ErrorResponse("Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!result.Success)
            {
                return result.Message.Contains("no encontrada") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Eliminar una categoría
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Categoría eliminada exitosamente</response>
        /// <response code="400">No se puede eliminar (tiene productos asociados)</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.Success)
            {
                return result.Message.Contains("no encontrada") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener una categoría por ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Detalles de la categoría</returns>
        /// <response code="200">Categoría encontrada</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener todas las categorías con contador de productos
        /// </summary>
        /// <returns>Lista completa de categorías</returns>
        /// <response code="200">Lista de categorías</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtener lista simple de categorías (solo ID y nombre)
        /// </summary>
        /// <returns>Lista simplificada de categorías</returns>
        /// <response code="200">Lista simple de categorías</response>
        [HttpGet("simple")]
        [ProducesResponseType(typeof(ApiResponse<List<CategorySimpleDto>>), 200)]
        public async Task<ActionResult<ApiResponse<List<CategorySimpleDto>>>> GetCategoriesSimple()
        {
            var result = await _categoryService.GetCategoriesSimpleAsync();
            return Ok(result);
        }
    }
}
