using Microsoft.AspNetCore.Mvc;
using Orders.Core.Interfaces;
using Orders.Core.Models.Dto;

namespace Orders.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("restaurant-fee")]
    public async Task<ActionResult> PayRestaurantFeeAsync(PayDto dto)
    {
        var response = await _paymentService.PayRestaurantFeeAsync(dto);
        return Ok(response);
    }
}