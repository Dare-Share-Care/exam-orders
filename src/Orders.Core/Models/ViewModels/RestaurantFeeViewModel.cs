using Orders.Infrastructure.Entities;

namespace Orders.Core.Models.ViewModels;

public class RestaurantFeeViewModel
{
    public decimal AmountPaid { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}