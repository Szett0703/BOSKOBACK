using DBTest_BACK.DTOs;

namespace DBTest_BACK.Services
{
    public interface ICategoryAdminService
    {
        Task<ApiResponse<List<CategoryResponseDto>>> GetCategoriesAsync();
        Task<ApiResponse<List<SimpleCategoryDto>>> GetSimpleCategoriesAsync();
        Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto dto);
        Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryCreateDto dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}
