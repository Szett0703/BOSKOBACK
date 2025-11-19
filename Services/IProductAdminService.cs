using DBTest_BACK.DTOs;

namespace DBTest_BACK.Services
{
    public interface IProductAdminService
    {
        Task<ApiResponse<PagedResponse<ProductListDto>>> GetProductsAsync(ProductFilterDto filters);
        Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto);
        Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductCreateDto dto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
    }
}
