using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SystemTools.SharedKernel;

namespace SystemTools.SystemToolsShared;

public interface IDatabaseAbstraction
{
    string GetTableName<T>() where T : class;
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<Result> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default);
    void SetCommandTimeout(TimeSpan timeout);
}
