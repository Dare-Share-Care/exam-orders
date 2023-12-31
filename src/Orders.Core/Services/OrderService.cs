using Microsoft.Extensions.Logging;
using Orders.Infrastructure.Entities;
using Orders.Core.Exceptions;
using Orders.Core.Interfaces;
using Orders.Infrastructure.Interfaces.Producers;
using Orders.Infrastructure.Interfaces;
using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;
using Orders.Core.Specifications;

namespace Orders.Core.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly ICatalogueService _catalogueService;
    private readonly ILoggingService _loggingService;
    private readonly IKafkaProducer _kafkaProducer;

    public OrderService(IRepository<Order> orderRepository, IReadRepository<Order> orderReadRepository,
        ICatalogueService catalogueService, IKafkaProducer kafkaProducer, ILoggingService loggingService)
    {
        _orderRepository = orderRepository;
        _orderReadRepository = orderReadRepository;
        _catalogueService = catalogueService;
        _kafkaProducer = kafkaProducer;
        _loggingService = loggingService;
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

    public async Task<List<OrderViewModel>> GetCustomersCompletedOrdersAsync(long userId)
    {
        var orders = await _orderReadRepository.ListAsync(new OrdersByUsersIdWithOrderLinesSpec(userId));
        //Filter orders by status (completed)
        var completedOrders = orders.Where(order => order.Status == OrderStatus.Completed).ToList();

        //Map the orders to the view model
        var orderViewModels = completedOrders.Select(order => new OrderViewModel
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

    public async Task<OrderViewModel> GetOrderAsync(long id)
    {
        var order = await _orderReadRepository.FirstOrDefaultAsync(new OrderAndOrderLinesSpec(id));


        if (order != null)
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

        if (catalogue == null)
        {
            await _loggingService.LogToFile(LogLevel.Error,
                "Customer tried to order from a restaurant that doesn't exist or service is down");
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
                TotalPrice = dto.Lines.Sum(line =>
                    catalogue.Menu.Single(item => item.Id == line.MenuItemId).Price * line.Quantity)
            };

            //Save order
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            
            //Calculate restaurant fee
            var fee = new RestaurantFee
            {
                OrderId = order.Id,
                RestaurantId = dto.RestaurantId,
                Amount = CalculateRestaurantFee(order.TotalPrice),
                PaymentStatus = 0 // 0 = Not paid
            };

            order.RestaurantFee = fee;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            
            //Email details
            //TODO: Add restaurant details containing menu items to email
            var emailDetails = new EmailDto()
            {
                To = dto.UserEmail,
                Subject = $"MTOGO - Order Confirmation",
                Body =
                    $"Your order has been placed and is being processed. Your order number is {order.Id},{System.Environment.NewLine}Thank you for using MTOGO!{System.Environment.NewLine}Best Regards,{System.Environment.NewLine}the MTOGO Team"
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

        await _loggingService.LogToFile(LogLevel.Information,
            "Customer tried to order items that doesn't exist in the catalogue");
        throw new InvalidMenuItemException("One or more items doesn't exist in the chosen catalogue");
    }

    private decimal CalculateRestaurantFee(decimal totalPrice)
    {
        // Switch case based on the total order price
        var feePercentage = totalPrice switch
        {
            < 101 => 0.06m,
            <= 1000 => 0.03m,
            _ => 0.03m
        };

        // Calculate the restaurant fee based on the total order price
        var restaurantFee = totalPrice * feePercentage;
        return restaurantFee;
    }
    public async Task<OrderViewModel> UpdateOrderStatusAsync(long orderId, OrderStatus status)
    {
        var order = await _orderReadRepository.FirstOrDefaultAsync(new OrderSpec(orderId));

        if (order != null)
        {
            var orderToUpdate = order.Status = status;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

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

        //Throw exception if order is not found
        await _loggingService.LogToFile(LogLevel.Information,
            "Customer tried to update order status on an order that doesn't exist");
        throw new OrderNotFoundException(orderId);
    }
}