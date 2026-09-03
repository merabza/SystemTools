using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SystemTools.Domain.Abstractions;

namespace SystemTools.RepositoriesShared;

public /*open*/ class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    protected UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public string GetTableName<T>() where T : class
    {
        IEntityType? entType = _dbContext.Model.GetEntityTypes().SingleOrDefault(s => s.ClrType == typeof(T));
        return entType?.GetTableName() ?? throw new Exception($"Table Name is null for {typeof(T).Name}");
    }

    public IEntityType? GetEntityTypeByTableName(string tableName)
    {
        return _dbContext.Model.GetEntityTypes().SingleOrDefault(w => w.GetTableName() == tableName);
    }

    //public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    //{
    //    return _dbContext.Database.BeginTransactionAsync(cancellationToken);
    //}

    public void SetCommandTimeout(TimeSpan timeout)
    {
        _dbContext.Database.SetCommandTimeout(timeout);
    }

    //public async Task<Option<ErrorOmd[]>> ExecuteSqlRawRetOptionAsync(string sql,
    //    CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    //        return null;
    //    }
    //    catch (Exception e)
    //    {
    //        return new[] { SystemToolsErrors.UnexpectedDatabaseException(e) };
    //    }
    //}
}
