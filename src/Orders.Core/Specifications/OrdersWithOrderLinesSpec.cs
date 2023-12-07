using Ardalis.Specification;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Specifications;

public sealed class OrdersAndOrderLinesSpec : Specification<Order>
{
    public OrdersAndOrderLinesSpec()
    {
        Query.Include(order => order.OrderLines)
            .Include(order => order.DeliveryAddress);
    }
}