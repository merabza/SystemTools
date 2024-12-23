using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using SystemToolsShared.Errors;

namespace RepositoriesDom;

public /*open*/ class AbstractRepository : IAbstractRepository
{
    private readonly DbContext _ctx;

    protected AbstractRepository(DbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<IDbContextTransaction> GetTransaction(CancellationToken cancellationToken = default)
    {
        return _ctx.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _ctx.SaveChangesAsync(cancellationToken);
    }

    public string? GetTableName<T>()
    {
        var entType = _ctx.Model.GetEntityTypes().SingleOrDefault(s => s.ClrType == typeof(T));
        return entType?.GetTableName();
    }

    public async Task<OneOf<int, Err[]>> ExecuteSqlRawRetOneOfAsync(string sql, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _ctx.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception e)
        {
            return new[] { SystemToolsErrors.UnexpectedDatabaseException(e) };
        }
    }

    protected async Task<Option<Err[]>> ExecuteSqlRawRetOptionAsync(string sql, CancellationToken cancellationToken = default)
    {
        try
        {
            await _ctx.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            return null;
        }
        catch (Exception e)
        {
            return new[] { SystemToolsErrors.UnexpectedDatabaseException(e) };
        }
    }
}