using Ardalis.Specification;
using Moq;
using Orders.Test.Helpers;
using Orders.Web.Entities;
using Orders.Web.Exceptions;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Interfaces.Repositories;
using Orders.Web.Models.Dto;
using Orders.Web.Models.Enums;
using Orders.Web.Models.ViewModels;
using Orders.Web.Producers;
using Orders.Web.Services;

namespace Orders.Test;

public class OrderServiceUnitTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IRepository<Order>> _mockOrderRepository = new();
    private readonly Mock<IReadRepository<Order>> _mockOrderReadRepository = new();
    private readonly Mock<ICatalogueService> _mockCatalogueService = new();
    private readonly Mock<KafkaProducer> _mockKafkaProducer = new();

    public OrderServiceUnitTests()
    {
        _orderService = new OrderService(_mockOrderRepository.Object, _mockOrderReadRepository.Object,
            _mockCatalogueService.Object, _mockKafkaProducer.Object);
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
        _mockOrderReadRepository
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), new CancellationToken()))
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

    [Fact]
    public async Task CreateOrderAsync_ReturnsOrder()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,

            Lines = new List<CreateOrderLineDto>
            {
                new CreateOrderLineDto()
                {
                    MenuItemId = 1,
                    Quantity = 1
                },
                new CreateOrderLineDto()
                {
                    MenuItemId = 2,
                    Quantity = 2
                }
            }
        };
        
        _mockCatalogueService.Setup(c => c.GetCatalogueAsync(It.IsAny<long>()))
            .ReturnsAsync(new CatalogueViewModel
            {
                RestaurantId = 1,
                Menu = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = 1, Name = "Item 1", Price = 100 },
                    new MenuItemViewModel { Id = 2, Name = "Item 2", Price = 150 }
                }
            });
        
        // Act
        var result = await _orderService.CreateOrderAsync(dto);

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(400, result.TotalPrice); //We expect a totl price of 400 (100 + 150 * 2)
    }
    
    [Fact]
    public async Task CreateOrderAsync_WhenInvalidMenuItems_ThrowsInvalidMenuItemException()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,

            Lines = new List<CreateOrderLineDto>
            {
                new CreateOrderLineDto()
                {
                    MenuItemId = 1,
                    Quantity = 1
                },
                new CreateOrderLineDto()
                {
                    MenuItemId = 3, //Invalid menu item, doesn't exist in the catalogue
                    Quantity = 2
                }
            }
        };
        
        _mockCatalogueService.Setup(c => c.GetCatalogueAsync(It.IsAny<long>()))
            .ReturnsAsync(new CatalogueViewModel
            {
                RestaurantId = 1,
                Menu = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = 1, Name = "Item 1", Price = 100 },
                    new MenuItemViewModel { Id = 2, Name = "Item 2", Price = 150 }
                }
            });
        
        //Act + Assert
        await Assert.ThrowsAsync<InvalidMenuItemException>(() => _orderService.CreateOrderAsync(dto));
    }
}