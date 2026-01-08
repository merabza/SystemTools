using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace RepositoriesAbstraction;

public interface IAbstractRepository
{
    Task<IDbContextTransaction> GetTransaction(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    string GetTableName<T>() where T : class;
    void ChangeCommandTimeOut(TimeSpan timeout);
}