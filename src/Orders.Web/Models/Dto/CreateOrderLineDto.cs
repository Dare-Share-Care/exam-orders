namespace Orders.Web.Models.Dto;

public class CreateOrderLineDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}