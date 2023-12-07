using Ardalis.Specification;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Specifications;

public sealed class OrderAndOrderLinesSpec : Specification<Order>
{
    public OrderAndOrderLinesSpec(long id)
    {
        Query
            .Where(o => o.Id == id)
            .Include(o => o.OrderLines)
            .Include(order => order.DeliveryAddress);
    }
}