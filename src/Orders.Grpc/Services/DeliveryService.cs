using System.Globalization;
using Grpc.Core;
using Orders.Core.Interfaces;

namespace Orders.Grpc.Services;

public class DeliveryService : Delivery.DeliveryBase
{
    private readonly IOrderService _orderService;

    public DeliveryService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<DeliveryResponse> SendOrdersForDelivery(DeliveryRequest request, ServerCallContext context)
    {
        var orders = await _orderService.GetInProgressOrdersAsync();
        
        //Convert orders to protobuf format
        var ordersMessage = orders.Select(order => new OrderMessage
        {
            Id = (int)order.Id,
            Status = order.Status.ToString(),
            TimeCreated = order.CreatedDate.ToString(CultureInfo.CurrentCulture),
            Address = new AddressMessage()
            {
                Street = order.DeliveryAddress.Street,
                City = order.DeliveryAddress.City,
                Zip = order.DeliveryAddress.ZipCode,
            }
        }).ToList();
        
        // Return the response
        return new DeliveryResponse() { Orders = {  ordersMessage }};
    }
}