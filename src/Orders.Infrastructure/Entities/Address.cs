namespace Orders.Infrastructure.Entities;

public class Address // ValueObject
{
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public int ZipCode { get; set; }
}