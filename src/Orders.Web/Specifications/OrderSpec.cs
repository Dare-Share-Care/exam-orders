using Ardalis.Specification;
using Orders.Web.Entities;

namespace Orders.Web.Specifications;

public sealed class OrderSpec : Specification<Order>
{
    public OrderSpec(long orderId)
    {
        Query.Where(order => order.Id == orderId);
    }    
}