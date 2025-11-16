using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    // ============================================
    // IMPLEMENTACIÓN: SERVICIO DE PRODUCTOS
    // ============================================
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IActivityLogService _activityLog;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, IActivityLogService activityLog, ILogger<ProductService> logger)
        {
            _context = context;
            _activityLog = activityLog;
            _logger = logger;
        }

        public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto)
        {
            try
            {
                // Validar que la categoría existe (si se proporciona)
                if (dto.CategoryId.HasValue)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
                    if (!categoryExists)
                    {
                        return ApiResponse<ProductResponseDto>.ErrorResponse("La categoría especificada no existe");
                    }
                }

                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    Image = dto.Image,
                    CategoryId = dto.CategoryId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("product", $"Producto creado: {product.Name}");

                var result = await GetProductByIdAsync(product.Id);
                return ApiResponse<ProductResponseDto>.SuccessResponse(result.Data!, "Producto creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return ApiResponse<ProductResponseDto>.ErrorResponse("Error al crear el producto");
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse("Producto no encontrado");
                }

                // Validar que la categoría existe (si se proporciona)
                if (dto.CategoryId.HasValue)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
                    if (!categoryExists)
                    {
                        return ApiResponse<ProductResponseDto>.ErrorResponse("La categoría especificada no existe");
                    }
                }

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Stock = dto.Stock;
                product.Image = dto.Image;
                product.CategoryId = dto.CategoryId;

                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("product", $"Producto actualizado: {product.Name}");

                var result = await GetProductByIdAsync(product.Id);
                return ApiResponse<ProductResponseDto>.SuccessResponse(result.Data!, "Producto actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {ProductId}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse("Error al actualizar el producto");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Producto no encontrado");
                }

                var productName = product.Name;
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Log de actividad
                await _activityLog.LogActivityAsync("product", $"Producto eliminado: {productName}");

                return ApiResponse<bool>.SuccessResponse(true, "Producto eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {ProductId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar el producto");
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse("Producto no encontrado");
                }

                var dto = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Image = product.Image,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    CreatedAt = product.CreatedAt
                };

                return ApiResponse<ProductResponseDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {ProductId}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse("Error al obtener el producto");
            }
        }

        public async Task<ApiResponse<PagedResponse<ProductListDto>>> GetProductsAsync(ProductFilterDto filter)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    query = query.Where(p => p.Name.Contains(filter.Search) || 
                                           (p.Description != null && p.Description.Contains(filter.Search)));
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
                }

                if (filter.InStock.HasValue)
                {
                    if (filter.InStock.Value)
                        query = query.Where(p => p.Stock > 0);
                    else
                        query = query.Where(p => p.Stock == 0);
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                // Ordenamiento
                query = filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "price" => filter.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "stock" => filter.SortDescending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
                    _ => filter.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var products = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(p => new ProductListDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Stock = p.Stock,
                        Image = p.Image,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ProductListDto>
                {
                    Items = products,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };

                return ApiResponse<PagedResponse<ProductListDto>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de productos");
                return ApiResponse<PagedResponse<ProductListDto>>.ErrorResponse("Error al obtener la lista de productos");
            }
        }

        public async Task<ApiResponse<List<ProductResponseDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => new ProductResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Stock = p.Stock,
                        Image = p.Image,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : null,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return ApiResponse<List<ProductResponseDto>>.SuccessResponse(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de categoría {CategoryId}", categoryId);
                return ApiResponse<List<ProductResponseDto>>.ErrorResponse("Error al obtener productos de la categoría");
            }
        }
    }
}
