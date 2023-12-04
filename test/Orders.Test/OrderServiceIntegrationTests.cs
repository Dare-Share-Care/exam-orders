using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orders.Test.Helpers;
using Orders.Web.Data;
using Orders.Web.Entities;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Models.Dto;
using Orders.Web.Models.ViewModels;
using Orders.Web.Producers;
using Orders.Web.Services;

namespace Orders.Test;

public class OrderServiceIntegrationTests : IDisposable
{
    private readonly OrderContext _context;

    public OrderServiceIntegrationTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .BuildServiceProvider();

        _context = serviceProvider.GetRequiredService<OrderContext>();
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var orderRepository = new EfRepository<Order>(_context);
        var catalogueService = new CatalogueService(new ConfigurationManager());
        var kafkaProducerMock = new Mock<KafkaProducer>();
        var orderService = new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object);
        
        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await _context.Orders.AddRangeAsync(testOrders);
        await _context.SaveChangesAsync();

        // Act
        var result = await orderService.GetOrdersAsync();
        
        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(3, result.Count); // We expect 3 orders
    }
    
    [Fact]
    public async Task GetInProgressOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var orderRepository = new EfRepository<Order>(_context);
        var catalogueService = new CatalogueService(new ConfigurationManager());
        var kafkaProducerMock = new Mock<KafkaProducer>();
        var orderService = new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object);
        
        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await _context.Orders.AddRangeAsync(testOrders);
        await _context.SaveChangesAsync();

        // Act
        var result = await orderService.GetInProgressOrdersAsync();
        
        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(2, result.Count); // We expect 2 orders, order 2 and 3
    }
    
    [Fact]
    public async Task GetOrderAsync_ReturnsOrder()
    {
        // Arrange
        var orderRepository = new EfRepository<Order>(_context);
        var catalogueService = new CatalogueService(new ConfigurationManager());
        var kafkaProducerMock = new Mock<KafkaProducer>();
        var orderService = new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object);
        
        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await _context.Orders.AddRangeAsync(testOrders);
        await _context.SaveChangesAsync();

        // Act
        var result = await orderService.GetOrderAsync(1);
        
        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(1, result.Id); // The Id of the order should be 1
    }
    
    [Fact]
    public async Task CreateOrderAsync_ReturnsOrder()
    {
        // Arrange
        var orderRepository = new EfRepository<Order>(_context);
        var catalogueServiceMock = new Mock<ICatalogueService>();
        var kafkaProducerMock = new Mock<KafkaProducer>();
        var orderService = new OrderService(orderRepository, orderRepository, catalogueServiceMock.Object, kafkaProducerMock.Object);
        
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
        
        //Mock CatalogueService (from Restaurant.Grpc microservice) 
        catalogueServiceMock.Setup(c => c.GetCatalogueAsync(It.IsAny<long>()))
            .ReturnsAsync(new CatalogueViewModel
            {
                RestaurantId = 1,
                Menu = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = 1, Name = "Item 1", Price = 100 },
                    new MenuItemViewModel { Id = 2, Name = "Item 2", Price = 150 }
                }
            });
        
        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await _context.Orders.AddRangeAsync(testOrders);
        await _context.SaveChangesAsync();

        // Act
        var result = await orderService.CreateOrderAsync(dto);
        
        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(4, result.Id); // The Id of the order should be 4
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}