using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using SystemTools.SystemToolsShared;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.RepositoriesShared;

public /*open*/ class DatabaseAbstractionRepository : IDatabaseAbstraction
{
    private readonly DbContext _dbContext;

    protected DatabaseAbstractionRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string GetTableName<T>() where T : class
    {
        IEntityType? entType = _dbContext.Model.GetEntityTypes().SingleOrDefault(s => s.ClrType == typeof(T));
        return entType?.GetTableName() ?? throw new Exception($"Table Name is null for {typeof(T).Name}");
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public void SetCommandTimeout(TimeSpan timeout)
    {
        _dbContext.Database.SetCommandTimeout(timeout);
    }

    public async Task<Option<Error[]>> ExecuteSqlRawRetOptionAsync(string sql,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            return null;
        }
        catch (Exception e)
        {
            return new[] { SystemToolsErrors.UnexpectedDatabaseException(e) };
        }
    }
}
