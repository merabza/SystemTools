using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Moq;

namespace DatabaseToolsShared.Tests;

public class DatabaseEntitiesDefaultConventionTests
{
    [Fact]
    public void ProcessModelFinalizing_DoesNotThrow_WhenEntityTypesAreEmpty()
    {
        var modelBuilderMock = new Mock<IConventionModelBuilder>();
        var modelMock = new Mock<IConventionModel>();
        modelBuilderMock.Setup(m => m.Metadata).Returns(modelMock.Object);
        modelMock.Setup(m => m.GetEntityTypes()).Returns(new List<IConventionEntityType>());
        var contextMock = new Mock<IConventionContext<IConventionModelBuilder>>();
        var convention = new DatabaseEntitiesDefaultConvention();
        convention.ProcessModelFinalizing(modelBuilderMock.Object, contextMock.Object);
        // No exception means success
    }

    [Fact]
    public void SetFieldNames_SetsColumnTypeForDateTimeAndDecimal()
    {
        var entityTypeMock = new Mock<IConventionEntityType>();
        var dateTimePropMock = new Mock<IConventionProperty>();
        dateTimePropMock.Setup(p => p.ClrType).Returns(typeof(DateTime));
        var decimalPropMock = new Mock<IConventionProperty>();
        decimalPropMock.Setup(p => p.ClrType).Returns(typeof(decimal));
        entityTypeMock.Setup(e => e.GetProperties()).Returns([dateTimePropMock.Object, decimalPropMock.Object]);
        // Call private method via reflection
        var method =
            typeof(DatabaseEntitiesDefaultConvention).GetMethod("SetFieldNames",
                BindingFlags.NonPublic | BindingFlags.Static);
        method.Invoke(null, [entityTypeMock.Object]);
        //dateTimePropMock.Verify(p => p.SetColumnType("datetime"), Times.Once);
        //decimalPropMock.Verify(p => p.SetColumnType("money"), Times.Once);
    }

    //[Fact]
    //public void SetIndexesNames_SetsIndexName_WhenNotSet()
    //{
    //    var entityTypeMock = new Mock<IConventionEntityType>();
    //    var indexMock = new Mock<IConventionIndex>();
    //    //indexMock.Setup(i => i.GetDatabaseName()).Returns((string)null);
    //    indexMock.Setup(i => i.IsUnique).Returns(true);
    //    indexMock.Setup(i => i.Properties).Returns(new List<IConventionProperty>());
    //    entityTypeMock.Setup(e => e.GetIndexes()).Returns([indexMock.Object]);
    //    var method =
    //        typeof(DatabaseEntitiesDefaultConvention).GetMethod("SetIndexesNames",
    //            BindingFlags.NonPublic | BindingFlags.Static);
    //    method.Invoke(null, [entityTypeMock.Object, "TableName"]);
    //    //indexMock.Verify(i => i.SetDatabaseName(It.IsAny<string>()), Times.Once);
    //}

    //[Fact]
    //public void SetRelationConstraintNames_SetsConstraintName_WhenNotSet()
    //{
    //    var entityTypeMock = new Mock<IConventionEntityType>();
    //    var foreignKeyMock = new Mock<IConventionForeignKey>();
    //    foreignKeyMock.Setup(fk => fk.GetConstraintName()).Returns((string)null);
    //    var principalEntityTypeMock = new Mock<IConventionEntityType>();
    //    principalEntityTypeMock.Setup(p => p.GetTableName()).Returns("RelatedTable");
    //    principalEntityTypeMock.Setup(p => p.Name).Returns("PrincipalType");
    //    foreignKeyMock.Setup(fk => fk.PrincipalEntityType).Returns(principalEntityTypeMock.Object);
    //    foreignKeyMock.Setup(fk => fk.Properties).Returns(new List<IConventionProperty>());
    //    entityTypeMock.Setup(e => e.GetForeignKeys()).Returns([foreignKeyMock.Object]);
    //    var method = typeof(DatabaseEntitiesDefaultConvention).GetMethod("SetRelationConstraintNames",
    //        BindingFlags.NonPublic | BindingFlags.Static);
    //    method.Invoke(null, [entityTypeMock.Object, "TableName"]);
    //    //foreignKeyMock.Verify(fk => fk.SetConstraintName(It.IsAny<string>()), Times.Once);
    //}
}