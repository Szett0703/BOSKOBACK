using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    /// <summary>
    /// Controlador para gestión de productos (Admin)
    /// </summary>
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin,Employee")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<AdminProductsController> _logger;

        public AdminProductsController(IProductService productService, ILogger<AdminProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Crear un nuevo producto
        /// </summary>
        /// <param name="dto">Datos del producto</param>
        /// <returns>Producto creado</returns>
        /// <response code="200">Producto creado exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductResponseDto>.ErrorResponse("Datos inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _productService.CreateProductAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Actualizar un producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Producto actualizado</returns>
        /// <response code="200">Producto actualizado exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductResponseDto>.ErrorResponse("Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _productService.UpdateProductAsync(id, dto);

            if (!result.Success)
            {
                return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Eliminar un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Producto eliminado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener un producto por ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Detalles del producto</returns>
        /// <response code="200">Producto encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener lista de productos con filtros y paginación
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de productos</returns>
        /// <response code="200">Lista de productos</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductListDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResponse<ProductListDto>>>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Obtener productos de una categoría específica
        /// </summary>
        /// <param name="categoryId">ID de la categoría</param>
        /// <returns>Lista de productos</returns>
        /// <response code="200">Lista de productos</response>
        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}
