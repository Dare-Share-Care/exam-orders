using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orders.Test.Drivers;
using Orders.Test.Helpers;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Entities;
using Orders.Infrastructure.Interfaces.Producers;
using Orders.Infrastructure.Interfaces;
using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;
using Orders.Core.Services;
using Orders.Core.Interfaces;


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
        var loggingServiceMock = new Mock<ILoggingService>();
        var catalogueService = new CatalogueService(new ConfigurationManager(), loggingServiceMock.Object);
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var orderService =
            new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object,
                loggingServiceMock.Object);

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
        var loggingServiceMock = new Mock<ILoggingService>();
        var catalogueService = new CatalogueService(new ConfigurationManager(), loggingServiceMock.Object);
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var orderService =
            new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object,
                loggingServiceMock.Object);

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
        var loggingServiceMock = new Mock<ILoggingService>();
        var catalogueService = new CatalogueService(new ConfigurationManager(), loggingServiceMock.Object);
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var orderService =
            new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object,
                loggingServiceMock.Object);

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
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var orderService = new OrderService(orderRepository, orderRepository, catalogueServiceMock.Object,
            kafkaProducerMock.Object, new Mock<ILoggingService>().Object);

        var dto = new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,
            DeliveryAddress = new DeliveryAddressDto()
            {
                Street = "Test street",
                City = "Test city",
                ZipCode = 1234
            },
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

    [Fact]
    public async Task CreateOrderAsync_SendsKafkaTopic()
    {
        // Arrange
        var orderRepositoryMock = new Mock<IRepository<Order>>();
        var orderReadRepositoryMock = new Mock<IReadRepository<Order>>();
        var catalogueServiceMock = new Mock<ICatalogueService>();
        var kafkaProducer = new TestProducer();
        var orderService =
            new OrderService
            (
                orderRepositoryMock.Object,
                orderReadRepositoryMock.Object, catalogueServiceMock.Object,
                kafkaProducer, new Mock<ILoggingService>().Object
            );

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
        //Setup kafka environment
        await TestTopicManager.CreateTopic("test-send-email");


        // Act
        await orderService.CreateOrderAsync(GetTestCreateOrderDto());

        //Get kafka topic message count
        var topicMessages = await TestTopicManager.GetTopicMessages("test-send-email");

        // Assert
        Assert.Single(topicMessages); // We expect 1 message in the topic
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ReturnsOrder_TestsDatabase()
    {
        // Arrange
        var orderRepository = new EfRepository<Order>(_context);
        var loggingServiceMock = new Mock<ILoggingService>();
        var catalogueService = new CatalogueService(new ConfigurationManager(), loggingServiceMock.Object);
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var orderService =
            new OrderService(orderRepository, orderRepository, catalogueService, kafkaProducerMock.Object,
                loggingServiceMock.Object);

        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await _context.Orders.AddRangeAsync(testOrders);
        await _context.SaveChangesAsync();

        // Act
        var result = await orderService.UpdateOrderStatusAsync(1, OrderStatus.InProgress);

        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(OrderStatus.InProgress, result.Status); // The status of the order should be InProgress
    }

    public async void Dispose()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();

        //Delete kafka topic after test
        await TestTopicManager.DeleteTopic("test-send-email");
    }

    private CreateOrderDto GetTestCreateOrderDto()
    {
        return new CreateOrderDto
        {
            RestaurantId = 1,
            UserId = 1,
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
}