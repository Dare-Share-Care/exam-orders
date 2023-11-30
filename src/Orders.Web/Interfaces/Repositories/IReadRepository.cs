using Ardalis.Specification;

namespace Orders.Web.Interfaces.Repositories;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
    
}