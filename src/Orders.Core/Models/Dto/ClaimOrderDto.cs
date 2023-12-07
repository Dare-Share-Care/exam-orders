using System.Text.Json.Serialization;

namespace Orders.Core.Models.Dto;

public class ClaimOrderDto
{
    [JsonPropertyName("to")]
    public long OrderId { get; set; }
}