using Microsoft.AspNetCore.Mvc;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Models.ViewModels;

namespace Orders.Web.Controllers;
    
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<List<OrderViewModel>>> GetOrdersAsync()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }
}