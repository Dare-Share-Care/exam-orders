using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;

namespace Orders.Core.Interfaces;

public interface IPaymentService
{
    Task<RestaurantFeeViewModel> PayRestaurantFeeAsync(PayDto dto);
}