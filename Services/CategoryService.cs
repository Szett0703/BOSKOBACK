using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    // ============================================
    // IMPLEMENTACIÓN: SERVICIO DE CATEGORÍAS
    // ============================================
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IActivityLogService _activityLog;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(AppDbContext context, IActivityLogService activityLog, ILogger<CategoryService> logger)
        {
            _context = context;
            _activityLog = activityLog;
            _logger = logger;
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto dto)
        {
            try
            {
                // Validar que no exista una categoría con el mismo nombre
                var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
                if (exists)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse("Ya existe una categoría con ese nombre");
                }

                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Image = dto.Image,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("category", $"Categoría creada: {category.Name}");

                var result = await GetCategoryByIdAsync(category.Id);
                return ApiResponse<CategoryResponseDto>.SuccessResponse(result.Data!, "Categoría creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Error al crear la categoría");
            }
        }

        public async Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse("Categoría no encontrada");
                }

                // Validar que no exista otra categoría con el mismo nombre
                var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);
                if (exists)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse("Ya existe una categoría con ese nombre");
                }

                category.Name = dto.Name;
                category.Description = dto.Description;
                category.Image = dto.Image;

                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("category", $"Categoría actualizada: {category.Name}");

                var result = await GetCategoryByIdAsync(category.Id);
                return ApiResponse<CategoryResponseDto>.SuccessResponse(result.Data!, "Categoría actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría {CategoryId}", id);
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Error al actualizar la categoría");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Categoría no encontrada");
                }

                // Verificar si tiene productos asociados
                if (category.Products.Any())
                {
                    return ApiResponse<bool>.ErrorResponse($"No se puede eliminar la categoría porque tiene {category.Products.Count} producto(s) asociado(s)");
                }

                var categoryName = category.Name;
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("category", $"Categoría eliminada: {categoryName}");

                return ApiResponse<bool>.SuccessResponse(true, "Categoría eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría {CategoryId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar la categoría");
            }
        }

        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse("Categoría no encontrada");
                }

                var dto = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image,
                    ProductCount = category.Products.Count,
                    CreatedAt = category.CreatedAt
                };

                return ApiResponse<CategoryResponseDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría {CategoryId}", id);
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Error al obtener la categoría");
            }
        }

        public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Image = c.Image,
                        ProductCount = c.Products.Count,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return ApiResponse<List<CategoryResponseDto>>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de categorías");
                return ApiResponse<List<CategoryResponseDto>>.ErrorResponse("Error al obtener la lista de categorías");
            }
        }

        public async Task<ApiResponse<List<CategorySimpleDto>>> GetCategoriesSimpleAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .OrderBy(c => c.Name)
                    .Select(c => new CategorySimpleDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProductCount = c.Products.Count
                    })
                    .ToListAsync();

                return ApiResponse<List<CategorySimpleDto>>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista simple de categorías");
                return ApiResponse<List<CategorySimpleDto>>.ErrorResponse("Error al obtener la lista de categorías");
            }
        }
    }
}
