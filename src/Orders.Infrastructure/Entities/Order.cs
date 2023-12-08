namespace Orders.Infrastructure.Entities;

public class Order : BaseEntity
{
    public long UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public Address DeliveryAddress { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderLine> OrderLines { get; set; } = new();
    public RestaurantFee RestaurantFee { get; set; } = null!;
}