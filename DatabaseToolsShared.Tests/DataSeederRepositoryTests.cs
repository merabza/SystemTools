//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace DatabaseToolsShared.Tests;

//public sealed class DataSeederRepositoryTests
//{
//    private Mock<DbContext> CreateDbContextMock(List<DummyEntity>? setData = null)
//    {
//        var dbSetMock = new Mock<DbSet<DummyEntity>>();
//        var data = setData ?? [];
//        var queryable = data.AsQueryable();
//        dbSetMock.As<IQueryable<DummyEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
//        dbSetMock.As<IQueryable<DummyEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
//        dbSetMock.As<IQueryable<DummyEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
//        // ReSharper disable once using
//        using var enumerator = queryable.GetEnumerator();
//        dbSetMock.As<IQueryable<DummyEntity>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
//        var dbContextMock = new Mock<DbContext>();
//        dbContextMock.Setup(c => c.Set<DummyEntity>()).Returns(dbSetMock.Object);
//        return dbContextMock;
//    }

//    [Fact]
//    public void GetAll_ReturnsAllEntities()
//    {
//        var entities = new List<DummyEntity> { new() { Id = 1 }, new() { Id = 2 } };
//        var dbContextMock = CreateDbContextMock(entities);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.GetAll<DummyEntity>();
//        Assert.Equal(2, result.Count);
//        Assert.Contains(result, e => e.Id == 1);
//        Assert.Contains(result, e => e.Id == 2);
//    }

//    [Fact]
//    public void HaveAnyRecord_ReturnsTrueIfAny()
//    {
//        var entities = new List<DummyEntity> { new() { Id = 1 } };
//        var dbContextMock = CreateDbContextMock(entities);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        Assert.True(repo.HaveAnyRecord<DummyEntity>());
//    }

//    [Fact]
//    public void HaveAnyRecord_ReturnsFalseIfNone()
//    {
//        var dbContextMock = CreateDbContextMock();
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        Assert.False(repo.HaveAnyRecord<DummyEntity>());
//    }

//    [Fact]
//    public void CreateEntities_AddsEntitiesAndSavesChanges()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.AddRange(It.IsAny<IEnumerable<DummyEntity>>()));
//        dbContextMock.Setup(c => c.SaveChanges()).Returns(1);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.CreateEntities(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.True(result);
//    }

//    [Fact]
//    public void CreateEntities_ReturnsTrueIfEmptyList()
//    {
//        var dbContextMock = CreateDbContextMock();
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.CreateEntities(new List<DummyEntity>());
//        Assert.True(result);
//    }

//    [Fact]
//    public void CreateEntities_ReturnsFalseOnException()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.AddRange(It.IsAny<IEnumerable<DummyEntity>>())).Throws(new Exception("fail"));
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.CreateEntities(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.False(result);
//    }

//    [Fact]
//    public void DeleteEntities_RemovesEntitiesAndSavesChanges()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.Remove(It.IsAny<DummyEntity>()));
//        dbContextMock.Setup(c => c.SaveChanges()).Returns(1);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.DeleteEntities(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.True(result);
//    }

//    [Fact]
//    public void DeleteEntities_ReturnsTrueIfEmptyList()
//    {
//        var dbContextMock = CreateDbContextMock();
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.DeleteEntities(new List<DummyEntity>());
//        Assert.True(result);
//    }

//    [Fact]
//    public void DeleteEntities_ReturnsFalseOnException()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.Remove(It.IsAny<DummyEntity>())).Throws(new Exception("fail"));
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.DeleteEntities(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.False(result);
//    }

//    [Fact]
//    public void SaveChanges_ReturnsTrueOnSuccess()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.SaveChanges()).Returns(1);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        Assert.True(repo.SaveChanges());
//    }

//    [Fact]
//    public void SaveChanges_ReturnsFalseOnException()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.SaveChanges()).Throws(new Exception("fail"));
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        Assert.False(repo.SaveChanges());
//    }

//    [Fact]
//    public void SetUpdates_UpdatesEntitiesAndSavesChanges()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.Update(It.IsAny<DummyEntity>()));
//        dbContextMock.Setup(c => c.SaveChanges()).Returns(1);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.SetUpdates(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.True(result);
//    }

//    [Fact]
//    public void SetUpdates_ReturnsTrueIfEmptyList()
//    {
//        var dbContextMock = CreateDbContextMock();
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.SetUpdates(new List<DummyEntity>());
//        Assert.True(result);
//    }

//    [Fact]
//    public void SetUpdates_ReturnsFalseOnException()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.Update(It.IsAny<DummyEntity>())).Throws(new Exception("fail"));
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.SetUpdates(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.False(result);
//    }

//    [Fact]
//    public void RemoveNeedlessRecords_RemovesEntitiesAndSavesChanges()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.RemoveRange(It.IsAny<IEnumerable<DummyEntity>>()));
//        dbContextMock.Setup(c => c.SaveChanges()).Returns(1);
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.RemoveNeedlessRecords(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.True(result);
//    }

//    [Fact]
//    public void RemoveNeedlessRecords_ReturnsTrueIfEmptyList()
//    {
//        var dbContextMock = CreateDbContextMock();
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.RemoveNeedlessRecords(new List<DummyEntity>());
//        Assert.True(result);
//    }

//    [Fact]
//    public void RemoveNeedlessRecords_ReturnsFalseOnException()
//    {
//        var dbContextMock = CreateDbContextMock();
//        dbContextMock.Setup(c => c.RemoveRange(It.IsAny<IEnumerable<DummyEntity>>())).Throws(new Exception("fail"));
//        var loggerMock = new Mock<ILogger<DataSeederRepository>>();
//        var repo = new DataSeederRepository(dbContextMock.Object, loggerMock.Object);
//        var result = repo.RemoveNeedlessRecords(new List<DummyEntity> { new() { Id = 1 } });
//        Assert.False(result);
//    }

//    public sealed class DummyEntity
//    {
//        public int Id { get; set; }
//    }
//}