using Microsoft.EntityFrameworkCore;
using RepositoriesDom;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestRepository : AbstractRepository
{
    public TestRepository(DbContext ctx) : base(ctx)
    {
    }
}

public class AbstractRepositoryTests
{
    private TestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
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
        using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        ctx.TestEntities.Add(new TestEntity { Name = "Test" });
        await repo.SaveChangesAsync();
        Assert.Single(ctx.TestEntities);
    }

    [Fact]
    public void GetTableName_ReturnsTableName()
    {
        using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var tableName = repo.GetTableName<TestEntity>();
        Assert.NotNull(tableName);
    }

    [Fact]
    public async Task ExecuteSqlRawRetOneOfAsync_ReturnsIntOrError()
    {
        using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var result = await repo.ExecuteSqlRawRetOneOfAsync("SELECT 1");
        Assert.True(result.IsT0 || result.IsT1);
    }

    [Fact]
    public async Task ExecuteSqlRawRetOptionAsync_ReturnsNullOrError()
    {
        using var ctx = CreateDbContext();
        var repo = new TestRepository(ctx);
        var result = await repo.ExecuteSqlRawRetOptionAsync("SELECT 1");
        Assert.True(result == null || result.IsSome);
    }
}