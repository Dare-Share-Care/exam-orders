namespace Orders.Web.Models.ViewModels;

public class OrderViewModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderLineViewModel> OrderLines { get; set; }
}