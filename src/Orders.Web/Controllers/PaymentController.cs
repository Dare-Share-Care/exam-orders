using Microsoft.AspNetCore.Mvc;
using Orders.Core.Interfaces;

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
    
    [HttpPost("restaurant-fee/{id}")]
    public async Task<ActionResult> PayRestaurantFeeAsync(long id)
    {
        await _paymentService.PayRestaurantFeeAsync(id);
        return Ok();
    }
}