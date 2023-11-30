using Ardalis.Specification;
using Moq;
using Orders.Web.Entities;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Services;

namespace Orders.Test;

public class OrderServiceUnitTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IRepository<Order>> _mockOrderRepository = new();
    private readonly Mock<IReadRepository<Order>> _mockOrderReadRepository = new();

    public OrderServiceUnitTests()
    {
        _orderService = new OrderService(_mockOrderRepository.Object, _mockOrderReadRepository.Object);
    }


    [Fact]
    public async Task GetOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var testOrders = GetTestOrders();

        //Mock repository and specification
        _mockOrderReadRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _orderService.GetOrdersAsync();

        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(3, result.Count); // We expect 3 orders
    }


    private static List<Order> GetTestOrders()
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
            new Order { Id = 1, UserId = 1, CreatedDate = DateTime.Now, TotalPrice = 100, OrderLines = orderLines },
            new Order { Id = 2, UserId = 2, CreatedDate = DateTime.Now, TotalPrice = 200, OrderLines = orderLines },
            new Order { Id = 3, UserId = 3, CreatedDate = DateTime.Now, TotalPrice = 300, OrderLines = orderLines }
        };
        return orders;
    }
}