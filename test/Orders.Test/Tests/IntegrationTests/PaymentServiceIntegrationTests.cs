using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Core.Interfaces;
using Orders.Core.Services;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Entities;
using Orders.Core.Models.Dto;
using Moq;

namespace Orders.Test.Tests.IntegrationTests;

public class PaymentServiceIntegrationTests
{
    private readonly OrderContext _context;
    
    public PaymentServiceIntegrationTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .BuildServiceProvider();

        _context = serviceProvider.GetRequiredService<OrderContext>();
    }
    
    [Fact]
    public async Task PayRestaurantFeeAsync_ReturnsRestaurantFeeViewModel()
    {
        // Arrange
        var restaurantFeeRepository = new EfRepository<RestaurantFee>(_context);
        var loggingServiceMock = new Mock<ILoggingService>();
        var paymentService = new PaymentService(restaurantFeeRepository, loggingServiceMock.Object);
        
        // Seed data
        var testRestaurantFee = new RestaurantFee
        {
            Id = 1,
            Amount = 10,
            PaymentStatus = 0 //Unpaid
        };
        await _context.RestaurantFees.AddAsync(testRestaurantFee);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await paymentService.PayRestaurantFeeAsync(new PayDto { FeeId = 1 });

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(10, result.AmountPaid); //We expect 10
        Assert.Equal(PaymentStatus.Paid, result.PaymentStatus); //We expect Paid
    }
}