namespace SystemTools.Domain.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    //string GetTableName<T>() where T : class;
    //Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    //Task<Option<ErrorOmd[]>> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default);
    //void SetCommandTimeout(TimeSpan timeout);
}
