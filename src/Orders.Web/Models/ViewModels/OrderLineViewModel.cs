namespace Orders.Web.Models.ViewModels;

public class OrderLineViewModel
{
    public string MenuItemName { get; set; } = null!;
    public long MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}