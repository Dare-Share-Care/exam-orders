using Orders.Web.Entities;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Models.ViewModels;

namespace Orders.Web.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IReadRepository<Order> _orderReadRepository;

    public OrderService(IRepository<Order> orderRepository, IReadRepository<Order> orderReadRepository)
    {
        _orderRepository = orderRepository;
        _orderReadRepository = orderReadRepository;
    }

    public Task<List<OrderViewModel>> GetOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderViewModel> GetOrderAsync(int id)
    {
        throw new NotImplementedException();
    }
}