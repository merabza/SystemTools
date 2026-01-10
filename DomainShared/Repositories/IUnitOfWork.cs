using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DomainShared.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    string GetTableName<T>() where T : class;
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}