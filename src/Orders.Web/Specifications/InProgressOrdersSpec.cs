using Ardalis.Specification;
using Orders.Web.Entities;
using Orders.Web.Models.Enums;

namespace Orders.Web.Specifications;

public sealed class InProgressOrdersSpec : Specification<Order>
{
    public InProgressOrdersSpec()
    {
        Query.Where(order => order.Status == OrderStatus.InProgress)
            .Include(order => order.DeliveryAddress);
    }
}