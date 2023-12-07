using Orders.Infrastructure.Entities;

namespace Orders.Core.Models.ViewModels;

public class OrderToClaimViewModel
{
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DeliveryAddressViewModel DeliveryAddress { get; set; } = null!;
}