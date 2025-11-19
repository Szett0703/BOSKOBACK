using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    public class CategoryAdminService : ICategoryAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryAdminService> _logger;

        public CategoryAdminService(AppDbContext context, ILogger<CategoryAdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CategoryResponseDto>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new CategoryResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Image = c.Image,
                        ProductCount = c.Products.Count(),
                        IsActive = true,
                        CreatedAt = c.CreatedAt
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return ApiResponse<List<CategoryResponseDto>>.SuccessResponse(
                    categories,
                    "Categorías obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return ApiResponse<List<CategoryResponseDto>>.ErrorResponse(
                    "Error al obtener categorías",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<List<SimpleCategoryDto>>> GetSimpleCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new SimpleCategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return ApiResponse<List<SimpleCategoryDto>>.SuccessResponse(
                    categories,
                    "Categorías simples obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías simples");
                return ApiResponse<List<SimpleCategoryDto>>.ErrorResponse(
                    "Error al obtener categorías simples",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto dto)
        {
            try
            {
                // Verificar nombre único (case-insensitive)
                var exists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

                if (exists)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse(
                        $"Ya existe una categoría con el nombre '{dto.Name}'"
                    );
                }

                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description ?? string.Empty,
                    Image = dto.Image,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var response = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image,
                    ProductCount = 0,
                    IsActive = true,
                    CreatedAt = category.CreatedAt
                };

                _logger.LogInformation("Categoría creada: {Name} (ID: {Id})", category.Name, category.Id);

                return ApiResponse<CategoryResponseDto>.SuccessResponse(
                    response,
                    "Categoría creada exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return ApiResponse<CategoryResponseDto>.ErrorResponse(
                    "Error al crear la categoría",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryCreateDto dto)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse(
                        $"Categoría con ID {id} no encontrada"
                    );
                }

                // Verificar nombre único (excepto para la misma categoría)
                var exists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);

                if (exists)
                {
                    return ApiResponse<CategoryResponseDto>.ErrorResponse(
                        $"Ya existe otra categoría con el nombre '{dto.Name}'"
                    );
                }

                category.Name = dto.Name;
                category.Description = dto.Description ?? string.Empty;
                category.Image = dto.Image;

                await _context.SaveChangesAsync();

                var response = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image,
                    ProductCount = category.Products.Count,
                    IsActive = true,
                    CreatedAt = category.CreatedAt
                };

                _logger.LogInformation("Categoría actualizada: {Name} (ID: {Id})", category.Name, category.Id);

                return ApiResponse<CategoryResponseDto>.SuccessResponse(
                    response,
                    "Categoría actualizada exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría {Id}", id);
                return ApiResponse<CategoryResponseDto>.ErrorResponse(
                    "Error al actualizar la categoría",
                    ex.Message,
                    ex.StackTrace
                );
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
                    return ApiResponse<bool>.ErrorResponse(
                        $"Categoría con ID {id} no encontrada"
                    );
                }

                var productCount = category.Products.Count;

                // Opción A: Eliminación en cascada (actualizar productos a NULL)
                if (productCount > 0)
                {
                    foreach (var product in category.Products)
                    {
                        product.CategoryId = null;
                    }
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                var message = productCount > 0
                    ? $"Categoría eliminada exitosamente. {productCount} productos actualizados sin categoría."
                    : "Categoría eliminada exitosamente";

                _logger.LogInformation("Categoría eliminada: {Name} (ID: {Id}), {ProductCount} productos afectados",
                    category.Name, category.Id, productCount);

                return ApiResponse<bool>.SuccessResponse(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría {Id}", id);
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la categoría",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }
    }
}
