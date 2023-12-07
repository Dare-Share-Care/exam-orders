using System.Globalization;
using Grpc.Core;
using Orders.Core.Interfaces;

namespace Orders.Grpc.Services;

public class ReviewService : Review.ReviewBase
{
    private readonly IOrderService _orderService;

    public ReviewService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<ReviewResponse> SendOrdersForReview(ReviewRequest request, ServerCallContext context)
    {
        var orders = await _orderService.GetCustomersCompletedOrdersAsync(request.UserId);

        //Convert orders to protobuf format
        var ordersMessage = orders.Select(order => new ReviewOrderMessage
        {
            Id = (int)order.Id,
            UserId = (int)order.UserId,
            TimeCreated = order.CreatedDate.ToString(CultureInfo.CurrentCulture),
            Status = order.Status.ToString(),
            Total = (float)order.TotalPrice,
            OrderLines =
            {
                order.OrderLines.Select(orderLine => new ReviewOrderLineMessage
                {
                    MenuItemName = orderLine.MenuItemName,
                    MenuItemId = (int)orderLine.MenuItemId,
                    Quantity = orderLine.Quantity,
                    Price = (float)orderLine.Price,
                })
            },
        }).ToList();


        // Return the response
        return new ReviewResponse() { Orders = {  ordersMessage }};
    }
}