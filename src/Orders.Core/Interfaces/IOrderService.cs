using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Interfaces;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetOrdersAsync();
    Task<List<OrderViewModel>> GetCustomersCompletedOrdersAsync(long userId);
    Task<List<OrderToClaimViewModel>> GetInProgressOrdersAsync();
    Task<OrderViewModel> GetOrderAsync(long id);
    Task<OrderViewModel> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderViewModel> UpdateOrderStatusAsync(long orderId, OrderStatus status);
}