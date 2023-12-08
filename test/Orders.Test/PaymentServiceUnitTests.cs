using Moq;
using Orders.Core.Interfaces;
using Orders.Core.Services;
using Orders.Infrastructure.Entities;
using Orders.Infrastructure.Interfaces;

namespace Orders.Test;

public class PaymentServiceUnitTests
{
    private readonly IPaymentService _paymentService;
    private readonly Mock<IRepository<RestaurantFee>> _mockRestaurantFeeRepository = new();

    public PaymentServiceUnitTests()
    {
        _paymentService = new PaymentService(_mockRestaurantFeeRepository.Object);
    }
    
    [Fact]
    public async Task PayRestaurantFeeAsync_ReturnsRestaurantFeeViewModel()
    {
        // Arrange
        var testRestaurantFee = new RestaurantFee
        {
            Id = 1,
            Amount = 10,
            PaymentStatus = 0 //Unpaid
        };

        //Mock repository and specification
        _mockRestaurantFeeRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>(), new CancellationToken()))
            .ReturnsAsync(testRestaurantFee);

        // Act
        var result = await _paymentService.PayRestaurantFeeAsync(1);

        // Assert
        Assert.NotNull(result); //Test if null
        Assert.Equal(10, result.AmountPaid); //We expect 10
        Assert.Equal(PaymentStatus.Paid, result.PaymentStatus); //We expect Paid
    }
}