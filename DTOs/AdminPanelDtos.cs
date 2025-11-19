using System.ComponentModel.DataAnnotations;

namespace DBTest_BACK.DTOs
{
    // ============================================
    // DTOs DE PRODUCTOS
    // ============================================

    /// <summary>
    /// DTO para crear un producto
    /// </summary>
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [MaxLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "La imagen debe ser una URL válida")]
        public string? Image { get; set; }

        public int? CategoryId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un producto
    /// </summary>
    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [MaxLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "La imagen debe ser una URL válida")]
        public string? Image { get; set; }

        public int? CategoryId { get; set; }
    }

    /// <summary>
    /// DTO de respuesta de producto
    /// </summary>
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para listar productos con paginación
    /// </summary>
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool InStock => Stock > 0;
    }

    /// <summary>
    /// DTO para filtros de productos
    /// </summary>
    public class ProductFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public bool? InStock { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; } = "CreatedAt"; // Name, Price, Stock, CreatedAt
        public bool SortDescending { get; set; } = true;
    }

    // ============================================
    // DTOs DE CATEGORÍAS
    // ============================================

    /// <summary>
    /// DTO para crear una categoría
    /// </summary>
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "La imagen debe ser una URL válida")]
        public string? Image { get; set; }
    }

    /// <summary>
    /// DTO para actualizar una categoría
    /// </summary>
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "La imagen debe ser una URL válida")]
        public string? Image { get; set; }
    }

    /// <summary>
    /// DTO de respuesta de categoría
    /// </summary>
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Image { get; set; }
        public int ProductCount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO simple de categoría para listas
    /// </summary>
    public class CategorySimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// DTO simple de categoría para dropdowns (solo ID y nombre)
    /// </summary>
    public class SimpleCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ============================================
    // DTOs DE USUARIOS (ADMIN)
    // ============================================

    /// <summary>
    /// DTO para actualizar usuario (Admin)
    /// </summary>
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [MaxLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "El teléfono no puede exceder 50 caracteres")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [MaxLength(50, ErrorMessage = "El rol no puede exceder 50 caracteres")]
        public string Role { get; set; } = "Customer"; // Admin, Employee, Customer

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO de respuesta de usuario (Admin)
    /// </summary>
    public class UserAdminResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    /// <summary>
    /// DTO para listar usuarios
    /// </summary>
    public class UserListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para filtros de usuarios
    /// </summary>
    public class UserFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Role { get; set; } // Admin, Employee, Customer
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "CreatedAt"; // Name, Email, CreatedAt
        public bool SortDescending { get; set; } = true;
    }

    // ============================================
    // DTOs DE ACTIVITY LOGS
    // ============================================

    // ============================================
    // DTOs GENÉRICOS
    // ============================================

    /// <summary>
    /// Respuesta estándar de la API
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public string? Error { get; set; }
        public string? StackTrace { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, string? error, string? stackTrace = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Error = error,
                StackTrace = stackTrace
            };
        }
    }

    /// <summary>
    /// Respuesta paginada
    /// </summary>
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
