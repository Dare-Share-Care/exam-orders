using System.ComponentModel.DataAnnotations;

namespace Orders.Web.Models.Dto;

public class EmailDto
{
    [Required] public string To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}