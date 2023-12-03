using Microsoft.AspNetCore.Mvc;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Models.Dto;
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

    [HttpGet("ready-for-delivery")]
    public async Task<ActionResult<List<OrderViewModel>>> GetOrdersReadyForDeliveryAsync()
    {
        var orders = await _orderService.GetInProgressOrdersAsync();
        return Ok(orders);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderViewModel>> GetOrderAsync(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        return Ok(order);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<OrderViewModel>> CreateOrderAsync([FromBody] CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return Ok(order);
    }
}