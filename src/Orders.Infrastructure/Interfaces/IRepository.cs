using Ardalis.Specification;

namespace Orders.Infrastructure.Interfaces;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
    
}