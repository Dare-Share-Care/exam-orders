using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Core.Interfaces;
using Orders.Core.Models.Dto;
using Orders.Core.Models.ViewModels;

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
    [Authorize(Roles="Admin")]
    public async Task<ActionResult<List<OrderViewModel>>> GetOrdersAsync()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<ActionResult<OrderViewModel>> GetOrderAsync(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        return Ok(order);
    }
    
    [HttpPost("create")]
    [Authorize(Roles="Customer")]
    public async Task<ActionResult<OrderViewModel>> CreateOrderAsync([FromBody] CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return Ok(order);
    }
}