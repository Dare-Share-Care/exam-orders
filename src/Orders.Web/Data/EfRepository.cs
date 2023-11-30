using Ardalis.Specification.EntityFrameworkCore;
using Orders.Web.Interfaces.Repositories;

namespace Orders.Web.Data;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class
{
    public readonly OrderContext OrderContext;

    public EfRepository(OrderContext orderContext) : base(orderContext) =>
        this.OrderContext = orderContext;
}