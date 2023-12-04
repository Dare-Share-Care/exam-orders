using Orders.Web.Entities;
using Orders.Web.Exceptions;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Producers;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Models.Dto;
using Orders.Web.Models.Enums;
using Orders.Web.Models.ViewModels;
using Orders.Web.Specifications;

namespace Orders.Web.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly ICatalogueService _catalogueService;
    private readonly IKafkaProducer _kafkaProducer;

    public OrderService(IRepository<Order> orderRepository, IReadRepository<Order> orderReadRepository, ICatalogueService catalogueService, IKafkaProducer kafkaProducer)
    {
        _orderRepository = orderRepository;
        _orderReadRepository = orderReadRepository;
        _catalogueService = catalogueService;
        _kafkaProducer = kafkaProducer;
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
                MenuItemName = orderLine.MenuItemName!,
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
                    MenuItemName = orderLine.MenuItemName!,
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

    public async Task<OrderViewModel> CreateOrderAsync(CreateOrderDto dto)
    {
        //Get the catalogue from the selected restaurant, must have RestaurantService running
        var catalogue = await _catalogueService.GetCatalogueAsync(dto.RestaurantId);
        
        if(catalogue == null)
        {
            throw new InvalidCatalogueException("Restaurant doesn't exist or service is down");
        }
        
        //Validate items exists in the catalogue
        var itemIds = catalogue.Menu.Select(item => item.Id).ToList();
        var itemsExists = dto.Lines.All(orderLine => itemIds.Contains(orderLine.MenuItemId));
        
        //Create the order
        if (itemsExists)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                CreatedDate = DateTime.UtcNow,
                Status = OrderStatus.New,
                DeliveryAddress = new Address
                {
                    Street = dto.DeliveryAddress.Street,
                    City = dto.DeliveryAddress.City,
                    ZipCode = dto.DeliveryAddress.ZipCode
                },
                OrderLines = dto.Lines.Select(line => new OrderLine
                {
                    MenuItemId = line.MenuItemId,
                    MenuItemName = catalogue.Menu.Single(item => item.Id == line.MenuItemId).Name,
                    Price = catalogue.Menu.Single(item => item.Id == line.MenuItemId).Price * line.Quantity,
                    Quantity = line.Quantity
                }).ToList(),
                TotalPrice = dto.Lines.Sum(line => catalogue.Menu.Single(item => item.Id == line.MenuItemId).Price * line.Quantity)
            };
            
            //Save order
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            
            //Email details
            var emailDetails = new EmailDto()
            {
                To = "user@example.com", //TODO: Get user email from auth service
                Subject = $"MTOGO - Order Confirmation",
                Body = $"Your order has been placed and is being processed. Your order id is {order.Id}, {System.Environment.NewLine} Thank you for using MTOGO!"
            };
            
            //Send email
            await _kafkaProducer.ProduceAsync("mtogo-send-email", emailDetails);
            
            //Map the order to the view model
            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedDate = order.CreatedDate,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                OrderLines = order.OrderLines.Select(orderLine => new OrderLineViewModel
                {
                    MenuItemName = orderLine.MenuItemName!,
                    MenuItemId = orderLine.MenuItemId,
                    Quantity = orderLine.Quantity,
                    Price = orderLine.Price
                }).ToList()
            };
            
            return orderViewModel;
        }
        throw new InvalidMenuItemException("One or more items doesn't exist in the chosen catalogue");
    }
}