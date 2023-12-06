using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Orders.Web.Entities;

namespace Orders.Web.Specifications;

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