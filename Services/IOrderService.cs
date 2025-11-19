using DBTest_BACK.DTOs;

namespace DBTest_BACK.Services
{
    public interface IOrderService
    {
        // CRUD Operations
        Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(OrderCreateDto dto);
        Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<PagedResponse<OrderListDto>>> GetOrdersAsync(OrderFilterDto filters);
        Task<ApiResponse<OrderResponseDto>> UpdateOrderStatusAsync(int id, OrderStatusUpdateDto dto);
        Task<ApiResponse<OrderResponseDto>> UpdateOrderAsync(int id, OrderUpdateDto dto);
        Task<ApiResponse<bool>> CancelOrderAsync(int id, string reason);

        // Customer Orders
        Task<ApiResponse<List<OrderListDto>>> GetCustomerOrdersAsync(int customerId);

        // Statistics
        Task<ApiResponse<OrderStatsDto>> GetOrderStatsAsync();

        // Helpers
        string GenerateOrderNumber();
    }
}
