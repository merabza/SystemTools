using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore.Storage;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.SystemToolsShared;

public interface IDatabaseAbstraction
{
    string GetTableName<T>() where T : class;
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<Option<Error[]>> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default);
    void SetCommandTimeout(TimeSpan timeout);
}
