using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Orders.Core.Models.Dto;

public class EmailDto
{
    [Required] [JsonPropertyName("to")]public string To { get; set; }
    [JsonPropertyName("subject")]public string? Subject { get; set; }
    [JsonPropertyName("body")]public string? Body { get; set; }
}