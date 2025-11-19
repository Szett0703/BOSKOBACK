using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    public class ProductAdminService : IProductAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductAdminService> _logger;

        public ProductAdminService(AppDbContext context, ILogger<ProductAdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResponse<ProductListDto>>> GetProductsAsync(ProductFilterDto filters)
        {
            try
            {
                // Validar y ajustar parámetros
                if (filters.PageSize > 100) filters.PageSize = 100;
                if (filters.PageSize < 1) filters.PageSize = 10;
                if (filters.Page < 1) filters.Page = 1;

                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Filtro de búsqueda (nombre o descripción)
                if (!string.IsNullOrWhiteSpace(filters.Search))
                {
                    var searchLower = filters.Search.ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(searchLower) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchLower))
                    );
                }

                // Filtro por categoría
                if (filters.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filters.CategoryId.Value);
                }

                // Filtro de stock
                if (filters.InStock.HasValue)
                {
                    if (filters.InStock.Value)
                    {
                        query = query.Where(p => p.Stock > 0);
                    }
                    else
                    {
                        query = query.Where(p => p.Stock == 0);
                    }
                }

                // Filtro de precio mínimo
                if (filters.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filters.MinPrice.Value);
                }

                // Filtro de precio máximo
                if (filters.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filters.MaxPrice.Value);
                }

                // Ordenamiento
                query = filters.SortBy.ToLower() switch
                {
                    "name" => filters.SortDescending
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),
                    "price" => filters.SortDescending
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
                    "stock" => filters.SortDescending
                        ? query.OrderByDescending(p => p.Stock)
                        : query.OrderBy(p => p.Stock),
                    _ => filters.SortDescending
                        ? query.OrderByDescending(p => p.CreatedAt)
                        : query.OrderBy(p => p.CreatedAt)
                };

                // Contar total antes de paginar
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((filters.Page - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .Select(p => new ProductListDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Stock = p.Stock,
                        Image = p.Image,
                        CategoryId = p.CategoryId ?? 0,
                        CategoryName = p.Category != null ? p.Category.Name : "Sin categoría",
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize);

                var response = new PagedResponse<ProductListDto>
                {
                    Items = items,
                    Page = filters.Page,
                    CurrentPage = filters.Page,
                    PageSize = filters.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

                return ApiResponse<PagedResponse<ProductListDto>>.SuccessResponse(
                    response,
                    "Productos obtenidos exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return ApiResponse<PagedResponse<ProductListDto>>.ErrorResponse(
                    "Error al obtener productos",
                    ex.Message,
                    ex.StackTrace
                );
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
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Producto con ID {id} no encontrado"
                    );
                }

                var response = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Image = product.Image,
                    CategoryId = product.CategoryId ?? 0,
                    CategoryName = product.Category != null ? product.Category.Name : "Sin categoría",
                    CreatedAt = product.CreatedAt
                };

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    response,
                    "Producto obtenido exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "Error al obtener el producto",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto)
        {
            try
            {
                // Verificar nombre único (case-insensitive)
                var exists = await _context.Products
                    .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

                if (exists)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Ya existe un producto con el nombre '{dto.Name}'"
                    );
                }

                // Verificar que la categoría existe
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId);

                if (!categoryExists)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"La categoría con ID {dto.CategoryId} no existe"
                    );
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

                // Cargar la categoría para la respuesta
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                var response = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Image = product.Image,
                    CategoryId = product.CategoryId ?? 0,
                    CategoryName = product.Category?.Name ?? "Sin categoría",
                    CreatedAt = product.CreatedAt
                };

                _logger.LogInformation("Producto creado: {Name} (ID: {Id})", product.Name, product.Id);

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    response,
                    "Producto creado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "Error al crear el producto",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductCreateDto dto)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Producto con ID {id} no encontrado"
                    );
                }

                // Verificar nombre único (excepto para el mismo producto)
                var exists = await _context.Products
                    .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id);

                if (exists)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Ya existe otro producto con el nombre '{dto.Name}'"
                    );
                }

                // Verificar que la nueva categoría existe
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId);

                if (!categoryExists)
                {
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"La categoría con ID {dto.CategoryId} no existe"
                    );
                }

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Stock = dto.Stock;
                product.Image = dto.Image;
                product.CategoryId = dto.CategoryId;

                await _context.SaveChangesAsync();

                // Recargar la categoría si cambió
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                var response = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Image = product.Image,
                    CategoryId = product.CategoryId ?? 0,
                    CategoryName = product.Category?.Name ?? "Sin categoría",
                    CreatedAt = product.CreatedAt
                };

                _logger.LogInformation("Producto actualizado: {Name} (ID: {Id})", product.Name, product.Id);

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    response,
                    "Producto actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "Error al actualizar el producto",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Producto con ID {id} no encontrado"
                    );
                }

                // Opción A: Permitir eliminación siempre (soft delete o mantener historial)
                // Si el producto tiene órdenes, aún se puede eliminar
                // El campo ProductName en OrderItems mantiene el historial

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado: {Name} (ID: {Id})", product.Name, product.Id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Producto eliminado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el producto",
                    ex.Message,
                    ex.StackTrace
                );
            }
        }
    }
}
