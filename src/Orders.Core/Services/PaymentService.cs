using Orders.Core.Interfaces;
using Orders.Core.Models.ViewModels;
using Orders.Infrastructure.Entities;
using Orders.Infrastructure.Interfaces;

namespace Orders.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<RestaurantFee> _restaurantFeeRepository;

    public PaymentService(IRepository<RestaurantFee> restaurantFeeRepository)
    {
        _restaurantFeeRepository = restaurantFeeRepository;
    }

    public Task<RestaurantFeeViewModel> PayRestaurantFeeAsync(long feeId)
    {
        throw new NotImplementedException();
    }
}