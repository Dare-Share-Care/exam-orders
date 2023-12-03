using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Test.Helpers;
using Orders.Web.Data;
using Orders.Web.Entities;
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
        var orderService = new OrderService(orderRepository, orderRepository, catalogueService);
        
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
        var orderService = new OrderService(orderRepository, orderRepository, catalogueService);
        
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

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}