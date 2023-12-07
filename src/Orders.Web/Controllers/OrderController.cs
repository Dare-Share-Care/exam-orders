using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles="Admin")]
    public async Task<ActionResult<List<OrderViewModel>>> GetOrdersAsync()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("ready-for-delivery")]
    [Authorize(Roles="Courier")]
    public async Task<ActionResult<List<OrderViewModel>>> GetOrdersReadyForDeliveryAsync()
    {
        var orders = await _orderService.GetInProgressOrdersAsync();
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
    
    [HttpGet("customer/{id}")]
    [Authorize(Roles="Customer")]
    public async Task<ActionResult<OrderViewModel>> GetCompletedOrdersByUserId(int id)
    {
        var order = await _orderService.GetCustomersCompletedOrdersAsync(id);
        return Ok(order);
    }
}