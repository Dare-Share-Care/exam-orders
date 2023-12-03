using Orders.Web.Models.Dto;
using Orders.Web.Models.ViewModels;

namespace Orders.Web.Interfaces.DomainServices;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetOrdersAsync();
    Task<List<OrderToClaimViewModel>> GetInProgressOrdersAsync();
    Task<OrderViewModel> GetOrderAsync(int id);
    Task<OrderViewModel> CreateOrderAsync(CreateOrderDto dto);
}