using Ardalis.Specification;
using Orders.Infrastructure.Entities;

namespace Orders.Core.Specifications;

public sealed class OrderSpec : Specification<Order>
{
    public OrderSpec(long orderId)
    {
        Query.Where(order => order.Id == orderId);
    }    
}