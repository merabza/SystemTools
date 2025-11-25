using System;
using System.Collections.Generic;
using Moq;

namespace DatabaseToolsShared.Tests;

public sealed class DataSeederTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object, ESeedDataType.OnlyJson, ["Id"]);
        Assert.NotNull(seeder);
    }

    [Fact]
    public void LoadFromJsonFile_ReturnsEmptyList_WhenFileDoesNotExist()
    {
        var result =
            DataSeeder<DummyDst, DummyJMo>.LoadFromJsonFile<DummyJMo>("nonexistent_folder", "nonexistent_file.json");
        Assert.Empty(result);
    }

    [Fact]
    public void Adjust_ReturnsPrioritizedList()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object, ESeedDataType.OnlyJson, ["Id"]);
        var priority = new List<DummyDst> { new() { Id = 1, Name = "A" } };
        var secondary = new List<DummyDst> { new() { Id = 2, Name = "B" } };
        var result = seeder.Adjust(priority, secondary);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == 1);
        Assert.Contains(result, x => x.Id == 2);
    }

    [Fact]
    public void Adapt_MapsPropertiesCorrectly()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        var jsonList = new List<DummyJMo> { new() { Id = 1, Name = "Test" } };
        var result = seeder.Adapt(jsonList);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Test", result[0].Name);
    }

    [Fact]
    public void CheckRecordsExists_DelegatesToRepo()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        repoMock.Setup(r => r.HaveAnyRecord<DummyDst>()).Returns(true);
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        Assert.True(seeder.CheckRecordsExists());
    }

    [Fact]
    public void Create_ReturnsFalse_IfRecordsExist()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        repoMock.Setup(r => r.HaveAnyRecord<DummyDst>()).Returns(true);
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        var result = seeder.Create(false);
        Assert.False(result);
    }

    [Fact]
    public void Create_ReturnsTrue_IfSeedDataTypeIsNone()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        repoMock.Setup(r => r.HaveAnyRecord<DummyDst>()).Returns(false);
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object, ESeedDataType.None);
        var result = seeder.Create(false);
        Assert.True(result);
    }

    [Fact]
    public void Create_ThrowsException_IfCreateEntitiesFails()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        repoMock.Setup(r => r.HaveAnyRecord<DummyDst>()).Returns(false);
        repoMock.Setup(r => r.CreateEntities(It.IsAny<List<DummyDst>>())).Returns(false);
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        Assert.Throws<Exception>(() => seeder.Create(false));
    }

    [Fact]
    public void AdditionalCheck_Default_ReturnsTrue()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        Assert.True(seeder.AdditionalCheck(new List<DummyJMo>(), new List<DummyDst>()));
    }

    [Fact]
    public void CreateListByRules_Default_ReturnsEmptyList()
    {
        var repoMock = new Mock<IDataSeederRepository>();
        repoMock.Setup(r => r.GetTableName<DummyDst>()).Returns("DummyDst");
        var seeder = new DataSeeder<DummyDst, DummyJMo>("folder", repoMock.Object);
        var result = seeder.CreateListByRules();
        Assert.Empty(result);
    }

    public sealed class DummyDst
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class DummyJMo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}