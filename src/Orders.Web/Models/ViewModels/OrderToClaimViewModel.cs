using Orders.Web.Models.Enums;

namespace Orders.Web.Models.ViewModels;

public class OrderToClaimViewModel
{
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public string DeliveryAddress { get; set; } = null!;
}