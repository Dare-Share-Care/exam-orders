using Orders.Web.Models.ViewModels;

namespace Orders.Web.Interfaces.DomainServices;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetOrdersAsync();
    Task<OrderViewModel> GetOrderAsync(int id);
}