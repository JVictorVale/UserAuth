namespace UserAuth.Domain.Contracts;

public interface IUnitOfWork
{
    Task<bool> Commit();
}