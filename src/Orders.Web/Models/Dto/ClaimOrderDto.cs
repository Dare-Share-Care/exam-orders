using System.Text.Json.Serialization;

namespace Orders.Web.Models.Dto;

public class ClaimOrderDto
{
    [JsonPropertyName("to")]
    public long OrderId { get; set; }
}