using Microsoft.EntityFrameworkCore.Metadata;

namespace SystemTools.Domain.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    string GetTableName<T>() where T : class;
    IEntityType? GetEntityTypeByTableName(string tableName);

    //Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    //Task<Option<ErrorOmd[]>> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default);
    void SetCommandTimeout(TimeSpan timeout);
}
