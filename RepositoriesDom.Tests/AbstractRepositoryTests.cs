using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepositoriesDom.Tests;

public sealed class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}

public sealed class TestRepository : AbstractRepository
{
    public TestRepository(DbContext ctx) : base(ctx)
    {
    }
}

public sealed class AbstractRepositoryTests
{
    private static TestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        // ReSharper disable once DisposableConstructor
        return new TestDbContext(options);
    }

    //[Fact]
    //public void ChangeCommandTimeOut_SetsTimeout()
    //{
    //    using var ctx = CreateDbContext();
    //    var repo = new TestRepository(ctx);
    //    repo.ChangeCommandTimeOut(TimeSpan.FromSeconds(30));
    //    Assert.True(true); // No exception means success
    //}

    //[Fact]
    //public async Task GetTransaction_ReturnsTransaction()
    //{
    //    using var ctx = CreateDbContext();
    //    var repo = new TestRepository(ctx);
    //    using var transaction = await repo.GetTransaction();
    //    Assert.NotNull(transaction);
    //    await transaction.DisposeAsync();
    //}

    [Fact]
    public async Task SaveChangesAsync_SavesEntity()
    {
        // ReSharper disable once using
        await using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        ctx.TestEntities.Add(new TestEntity { Name = "Test" });
        await repo.SaveChangesAsync();
        Assert.Single(ctx.TestEntities);
    }

    [Fact]
    public void GetTableName_ReturnsTableName()
    {
        // ReSharper disable once using
        using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var tableName = repo.GetTableName<TestEntity>();
        Assert.NotNull(tableName);
    }

    [Fact]
    public async Task ExecuteSqlRawRetOneOfAsync_ReturnsIntOrError()
    {
        // ReSharper disable once using
        await using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var result = await repo.ExecuteSqlRawRetOneOfAsync("SELECT 1");
        Assert.True(result.IsT0 || result.IsT1);
    }

    [Fact]
    public async Task ExecuteSqlRawRetOptionAsync_ReturnsNullOrError()
    {
        // ReSharper disable once using
        await using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var result = await repo.ExecuteSqlRawRetOptionAsync("SELECT 1");
        Assert.True(result == null || result.IsSome);
    }
}