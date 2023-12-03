using Ardalis.Specification;
using Moq;
using Orders.Test.Helpers;
using Orders.Web.Entities;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Models.Enums;
using Orders.Web.Services;

namespace Orders.Test;

public class OrderServiceUnitTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IRepository<Order>> _mockOrderRepository = new();
    private readonly Mock<IReadRepository<Order>> _mockOrderReadRepository = new();
    private readonly Mock<CatalogueService> _mockCatalogueService = new();

    public OrderServiceUnitTests()
    {
        _orderService = new OrderService(_mockOrderRepository.Object, _mockOrderReadRepository.Object, _mockCatalogueService.Object);
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var testOrders = OrderTestHelper.GetTestOrders();

        //Mock repository and specification
        _mockOrderReadRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _orderService.GetOrdersAsync();

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(3, result.Count); //We expect 3 orders
    }

    [Fact]
    public async Task GetInProgressOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var testOrders = OrderTestHelper.GetTestOrders()
            .Where(x => x.Status == OrderStatus.InProgress).ToList();

        //Mock repository and specification
        _mockOrderReadRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _orderService.GetInProgressOrdersAsync();

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(2, result.Count); //We expect 2 orders (order 2 and 3)
    }
    
    [Fact]
    public async Task GetOrderAsync_ReturnsOrder()
    {
        // Arrange
        var testOrders = OrderTestHelper.GetTestOrders();

        //Mock repository and specification
        _mockOrderReadRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
            .ReturnsAsync(testOrders[0]);

        // Act
        var result = await _orderService.GetOrderAsync(1);

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(1, result.Id); //We expect order with id 1
    }
    
    [Fact]
    public async Task GetOrderAsync_ReturnsNull()
    {
        // Arrange
        var testOrders = OrderTestHelper.GetTestOrders();

        //Mock repository and specification
        _mockOrderReadRepository.Setup(x => x.GetByIdAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
            .ReturnsAsync(testOrders[0]);

        // Act
        var result = await _orderService.GetOrderAsync(4); //There should not be an order with id 4

        // Assert
        Assert.Null(result); //Test if null
    }
}