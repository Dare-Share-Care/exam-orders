using Orders.Web.Models.Dto;
using Orders.Web.Models.Enums;
using Orders.Web.Models.ViewModels;

namespace Orders.Web.Interfaces.DomainServices;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetOrdersAsync();
    Task<List<OrderViewModel>> GetCustomersCompletedOrdersAsync(long userId);
    Task<List<OrderToClaimViewModel>> GetInProgressOrdersAsync();
    Task<OrderViewModel> GetOrderAsync(long id);
    Task<OrderViewModel> CreateOrderAsync(CreateOrderDto dto);
    Task UpdateOrderStatusAsync(long orderId, OrderStatus status);
}