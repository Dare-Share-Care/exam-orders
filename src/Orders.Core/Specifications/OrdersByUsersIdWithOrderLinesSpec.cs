using Ardalis.Specification;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Specifications;

public sealed class OrdersByUsersIdWithOrderLinesSpec : Specification<Order>
{
    public OrdersByUsersIdWithOrderLinesSpec(long userId)
    {
        Query
            .Where(order => order.UserId == userId)
            .Include(order => order.OrderLines)
            .Include(order => order.DeliveryAddress);
    }
}