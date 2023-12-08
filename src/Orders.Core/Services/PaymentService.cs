using Microsoft.Extensions.Logging;
using Orders.Core.Exceptions;
using Orders.Core.Interfaces;
using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;
using Orders.Infrastructure.Entities;
using Orders.Infrastructure.Interfaces;

namespace Orders.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<RestaurantFee> _restaurantFeeRepository;
    private readonly ILoggingService _loggingService;

    public PaymentService(IRepository<RestaurantFee> restaurantFeeRepository, ILoggingService loggingService)
    {
        _restaurantFeeRepository = restaurantFeeRepository;
        _loggingService = loggingService;
    }

    public async Task<RestaurantFeeViewModel> PayRestaurantFeeAsync(PayDto dto)
    {
        var restaurantFee = await _restaurantFeeRepository.GetByIdAsync(dto.FeeId);

        //Pay the fee
        if (restaurantFee != null)
        {
            restaurantFee.PaymentStatus = PaymentStatus.Paid;
            await _restaurantFeeRepository.UpdateAsync(restaurantFee);
            
            return new RestaurantFeeViewModel
            {
                AmountPaid = restaurantFee.Amount,
                PaymentStatus = restaurantFee.PaymentStatus
            };
        }
        
        await _loggingService.LogToFile(LogLevel.Error, $"Fee with id {dto.FeeId} not found");
        throw new FeeNotFoundException($"Fee with id {dto.FeeId} not found");
    }
}