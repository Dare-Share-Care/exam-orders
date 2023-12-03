namespace Orders.Web.Entities;

public class OrderLine : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public string? MenuItemName { get; set; }
    public long MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}