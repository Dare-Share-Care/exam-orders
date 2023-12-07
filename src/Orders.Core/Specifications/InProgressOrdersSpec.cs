using Ardalis.Specification;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Specifications;

public sealed class InProgressOrdersSpec : Specification<Order>
{
    public InProgressOrdersSpec()
    {
        Query.Where(order => order.Status == OrderStatus.InProgress)
            .Include(order => order.DeliveryAddress);
    }
}