using Orders.Web.Entities;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Models.Dto;
using Orders.Web.Models.ViewModels;
using Orders.Web.Specifications;
using RestaurantNamespace;


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

    public async Task<List<OrderViewModel>> GetOrdersAsync()
    {
        //Get orders with order lines from the database
        var orders = await _orderReadRepository.ListAsync(new OrdersAndOrderLinesSpec());

        //Map the orders to the view model
        var orderViewModels = orders.Select(order => new OrderViewModel
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedDate = order.CreatedDate,
            TotalPrice = order.TotalPrice,
            OrderLines = order.OrderLines.Select(orderLine => new OrderLineViewModel
            {
                MenuItemName = "TODO",
                MenuItemId = orderLine.MenuItemId,
                Quantity = orderLine.Quantity,
                Price = orderLine.Price
            }).ToList()
        }).ToList();

        return orderViewModels;
    }

    public async Task<List<OrderToClaimViewModel>> GetInProgressOrdersAsync()
    {
        var orders = await _orderReadRepository.ListAsync(new InProgressOrdersSpec());

        var orderViewModels = orders.Select(order => new OrderToClaimViewModel
        {
            Id = order.Id,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            DeliveryAddress = new DeliveryAddressViewModel
            {
                Street = order.DeliveryAddress.Street,
                City = order.DeliveryAddress.City,
                ZipCode = order.DeliveryAddress.ZipCode,
            }
        }).ToList();

        return orderViewModels;
    }

    public async Task<OrderViewModel> GetOrderAsync(int id)
    {
        var order = await _orderReadRepository.FirstOrDefaultAsync(new OrderAndOrderLinesSpec(id));
        
        
        if(order != null)
        {
            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedDate = order.CreatedDate,
                TotalPrice = order.TotalPrice,
                OrderLines = order.OrderLines.Select(orderLine => new OrderLineViewModel
                {
                    MenuItemName = "TODO",
                    MenuItemId = orderLine.MenuItemId,
                    Quantity = orderLine.Quantity,
                    Price = orderLine.Price
                }).ToList()
            };

            return orderViewModel;
        }
        
        //Return null if order is not found
        return null!;
    }

    public Task<OrderViewModel> CreateOrderAsync(CreateOrderDto dto)
    {
        throw new NotImplementedException();
    }
}