namespace Orders.Web.Models.Dto;

public class CreateOrderDto
{
    public long RestaurantId { get; set; }
    public long UserId { get; set; }
    public List<CreateOrderLineDto> Lines { get; set; } = new();
}