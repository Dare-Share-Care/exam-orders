using Ardalis.Specification;
using Orders.Web.Entities;

namespace Orders.Web.Specifications;

public sealed class OrderAndOrderLinesSpec : Specification<Order>
{
    public OrderAndOrderLinesSpec(int id)
    {
        Query
            .Where(o => o.Id == id)
            .Include(o => o.OrderLines);
    }
}