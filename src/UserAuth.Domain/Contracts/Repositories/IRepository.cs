using System.Linq.Expressions;
using UserAuth.Domain.Entities;

namespace UserAuth.Domain.Contracts.Repositories;

public interface IRepository<T> : IDisposable where T : BaseEntity,IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate);
    Task<bool> Any(Expression<Func<T, bool>> predicate);
}