using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Test.Helpers;
using Orders.Web.Data;
using Orders.Web.Entities;
using Orders.Web.Services;

namespace Orders.Test;

public class OrderServiceIntegrationTests
{
    [Fact]
    public async Task GetOrdersAsync_ReturnsListOfOrders()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(databaseName: "TestDB"))
            .BuildServiceProvider();
        
        var dbContext = serviceProvider.GetRequiredService<OrderContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        var orderRepository = new EfRepository<Order>(dbContext);
        var orderService = new OrderService(orderRepository, orderRepository);
        
        // Seed data
        var testOrders = OrderTestHelper.GetTestOrders();
        await dbContext.Orders.AddRangeAsync(testOrders);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await orderService.GetOrdersAsync();
        
        // Assert
        Assert.NotNull(result); // Test if null
        Assert.Equal(3, result.Count); // We expect 3 orders
    }
}