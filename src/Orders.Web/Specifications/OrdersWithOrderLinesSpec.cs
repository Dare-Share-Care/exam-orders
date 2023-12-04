using Ardalis.Specification;
using Orders.Web.Entities;

namespace Orders.Web.Specifications;

public sealed class OrdersAndOrderLinesSpec : Specification<Order>
{
    public OrdersAndOrderLinesSpec()
    {
        Query.Include(order => order.OrderLines);
    }
}