using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace RepositoriesDom;

public interface IAbstractRepository
{
    Task<IDbContextTransaction> GetTransaction(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    string GetTableName<T>();
    void ChangeCommandTimeOut(TimeSpan timeout);
}