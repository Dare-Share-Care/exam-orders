using Orders.Core.Models.ViewModels;

namespace Orders.Core.Interfaces;

public interface IPaymentService
{
    Task<RestaurantFeeViewModel> PayRestaurantFeeAsync(long feeId);
}