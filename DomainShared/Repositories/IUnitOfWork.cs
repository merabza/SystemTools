using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore.Storage;
using SystemToolsShared.Errors;

namespace DomainShared.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    string GetTableName<T>() where T : class;
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<Option<Err[]>> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default);

}