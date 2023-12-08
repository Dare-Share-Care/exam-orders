namespace Orders.Infrastructure.Entities;

public class RestaurantFee : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public long RestaurantId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}