using Orders.Web.Entities;
using Orders.Web.Models.Enums;

namespace Orders.Test.Helpers;

public static class OrderTestHelper
{
    internal static List<Order> GetTestOrders()
    {
        //Mock some orders with order lines
        var orderLines = new List<OrderLine>
        {
            new OrderLine { Id = 1, OrderId = 1, MenuItemId = 1, Quantity = 1, Price = 100 },
            new OrderLine { Id = 2, OrderId = 2, MenuItemId = 2, Quantity = 2, Price = 200 },
            new OrderLine { Id = 3, OrderId = 3, MenuItemId = 3, Quantity = 3, Price = 300 }
        };

        var orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                UserId = 1,
                CreatedDate = DateTime.Now,
                TotalPrice = 100,
                OrderLines = orderLines,
                Status = OrderStatus.New
            },
            new Order
            {
                Id = 2,
                UserId = 2,
                CreatedDate = DateTime.Now,
                TotalPrice = 200,
                OrderLines = orderLines,
                Status = OrderStatus.InProgress
            },
            new Order
            {
                Id = 3,
                UserId = 3,
                CreatedDate = DateTime.Now,
                TotalPrice = 300,
                OrderLines = orderLines,
                Status = OrderStatus.InProgress
            }
        };
        return orders;
    }
}