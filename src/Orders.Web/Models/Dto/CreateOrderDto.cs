namespace Orders.Web.Models.Dto;

public class CreateOrderDto
{
    public long RestaurantId { get; set; }
    public long UserId { get; set; }
    public string UserEmail { get; set; } = null!;
    public List<CreateOrderLineDto> Lines { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = null!;
}