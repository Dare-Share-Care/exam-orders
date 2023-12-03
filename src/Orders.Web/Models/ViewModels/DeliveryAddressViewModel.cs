namespace Orders.Web.Models.ViewModels;

public class DeliveryAddressViewModel
{
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public int ZipCode { get; set; }
}