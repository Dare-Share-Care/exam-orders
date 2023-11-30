using Ardalis.Specification;

namespace Orders.Web.Interfaces.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
    
}