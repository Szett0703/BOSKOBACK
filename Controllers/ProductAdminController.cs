using DBTest_BACK.DTOs;
using DBTest_BACK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBTest_BACK.Controllers
{
    /// <summary>
    /// Controlador para gestión de productos (Admin/Employee)
    /// </summary>
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin,Employee")]
    public class ProductAdminController : ControllerBase
    {
        private readonly IProductAdminService _productService;
        private readonly ILogger<ProductAdminController> _logger;

        public ProductAdminController(IProductAdminService productService, ILogger<ProductAdminController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener productos con paginación y filtros
        /// </summary>
        /// <param name="filters">Filtros de búsqueda</param>
        /// <returns>Lista paginada de productos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductListDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResponse<ProductListDto>>>> GetProducts([FromQuery] ProductFilterDto filters)
        {
            var result = await _productService.GetProductsAsync(filters);
            return Ok(result);
        }

        /// <summary>
        /// Obtener un producto por ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Detalles del producto</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Crear un nuevo producto
        /// </summary>
        /// <param name="dto">Datos del producto</param>
        /// <returns>Producto creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<ProductResponseDto>.ErrorResponse(
                    "Error de validación",
                    string.Join(", ", errors)
                ));
            }

            var result = await _productService.CreateProductAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProductById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Actualizar un producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Producto actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(int id, [FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<ProductResponseDto>.ErrorResponse(
                    "Error de validación",
                    string.Join(", ", errors)
                ));
            }

            var result = await _productService.UpdateProductAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Eliminar un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Confirmación de eliminación</returns>
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
    }
}
