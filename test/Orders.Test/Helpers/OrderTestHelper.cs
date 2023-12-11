using Orders.Core.Models.Dto;
using Orders.Infrastructure.Entities;

namespace Orders.Test.Helpers;

internal static class OrderTestHelper
{
    internal static List<Order> GetTestOrders()
    {
        //Mock some orders with order lines
        var orderLines = new List<OrderLine>
        {
            new() { Id = 1, OrderId = 1, MenuItemId = 1, Quantity = 1, Price = 100 },
            new() { Id = 2, OrderId = 2, MenuItemId = 2, Quantity = 2, Price = 200 },
            new() { Id = 3, OrderId = 3, MenuItemId = 3, Quantity = 3, Price = 300 }
        };

        var deliveryAddress = new Address
        {
            Street = "Test street",
            City = "Test city",
            ZipCode = 1234
        };

        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                CreatedDate = DateTime.Now,
                TotalPrice = 100,
                OrderLines = orderLines,
                Status = OrderStatus.Completed,
                DeliveryAddress = deliveryAddress
            },
            new()
            {
                Id = 2,
                UserId = 2,
                CreatedDate = DateTime.Now,
                TotalPrice = 200,
                OrderLines = orderLines,
                Status = OrderStatus.InProgress,
                DeliveryAddress = deliveryAddress
            },
            new()
            {
                Id = 3,
                UserId = 3,
                CreatedDate = DateTime.Now,
                TotalPrice = 300,
                OrderLines = orderLines,
                Status = OrderStatus.InProgress,
                DeliveryAddress = deliveryAddress
            }
        };
        return orders;
    }

    internal static CreateOrderDto GetTestCreateOrderDto()
    {
        return new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,
            UserEmail = "user@example.com",
            DeliveryAddress = new DeliveryAddressDto()
            {
                Street = "Test street",
                City = "Test city",
                ZipCode = 1234
            },
            Lines = new List<CreateOrderLineDto>
            {
                new()
                {
                    MenuItemId = 1,
                    Quantity = 1
                },
                new()
                {
                    MenuItemId = 2,
                    Quantity = 2
                }
            }
        };
    }
    
    internal static CreateOrderDto GetTestCreateOrderDtoInvalidMenuItems()
    {
        return new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,
            UserEmail = "user@example.com",
            DeliveryAddress = new DeliveryAddressDto()
            {
                Street = "Test street",
                City = "Test city",
                ZipCode = 1234
            },
            Lines = new List<CreateOrderLineDto>
            {
                new()
                {
                    MenuItemId = 99, //Invalid menu item
                    Quantity = 1
                },
                new()
                {
                    MenuItemId = 48, //Invalid menu item
                    Quantity = 2
                }
            }
        };
    }
}