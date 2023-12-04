namespace Orders.Web.Models.Dto;

public class DeliveryAddressDto
{
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public int ZipCode { get; set; }
}