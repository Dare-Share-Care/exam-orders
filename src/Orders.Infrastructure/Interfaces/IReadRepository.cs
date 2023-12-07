using Ardalis.Specification;

namespace Orders.Infrastructure.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
    
}