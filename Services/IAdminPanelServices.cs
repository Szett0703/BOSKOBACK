using DBTest_BACK.DTOs;

namespace DBTest_BACK.Services
{
    // ============================================
    // INTERFAZ: SERVICIO DE PRODUCTOS
    // ============================================
    public interface IProductService
    {
        Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto);
        Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductUpdateDto dto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<PagedResponse<ProductListDto>>> GetProductsAsync(ProductFilterDto filter);
        Task<ApiResponse<List<ProductResponseDto>>> GetProductsByCategoryAsync(int categoryId);
    }

    // ============================================
    // INTERFAZ: SERVICIO DE CATEGORÍAS
    // ============================================
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto dto);
        Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
        Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<List<CategorySimpleDto>>> GetCategoriesSimpleAsync();
    }

    // ============================================
    // INTERFAZ: SERVICIO DE USUARIOS (ADMIN)
    // ============================================
    public interface IUserAdminService
    {
        Task<ApiResponse<UserAdminResponseDto>> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<ApiResponse<bool>> ToggleUserStatusAsync(int id);
        Task<ApiResponse<bool>> DeleteUserAsync(int id);
        Task<ApiResponse<UserAdminResponseDto>> GetUserByIdAsync(int id);
        Task<ApiResponse<PagedResponse<UserListDto>>> GetUsersAsync(UserFilterDto filter);
        Task<ApiResponse<bool>> ChangeUserRoleAsync(int id, string role);
    }

    // ============================================
    // INTERFAZ: SERVICIO DE ACTIVITY LOGS
    // ============================================
    public interface IActivityLogService
    {
        Task LogActivityAsync(string type, string text, int? userId = null);
        Task<List<ActivityDto>> GetRecentActivityAsync(int limit = 10);
    }
}
