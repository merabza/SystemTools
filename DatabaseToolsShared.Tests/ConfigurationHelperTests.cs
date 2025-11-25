namespace DatabaseToolsShared.Tests;

public sealed class ConfigurationHelperTests
{
    [Theory]
    [InlineData("Table", false, new[] { "Field1" }, "IX_Table_field1")]
    [InlineData("Table", true, new[] { "Field1" }, "IX_Table_field1_Unique")]
    [InlineData("Table", false, new[] { "Field1", "Field2" }, "IX_Table_field1_field2")]
    [InlineData("Table", true, new[] { "Field1", "Field2" }, "IX_Table_field1_field2_Unique")]
    public void CreateIndexName_ReturnsExpected(string tableName, bool unique, string[] fieldNames, string expected)
    {
        var result = tableName.CreateIndexName(unique, fieldNames);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Table", "Related", "FK_Table_Relateds")]
    [InlineData("Person", "Child", "FK_Person_Children")]
    public void CreateConstraintName_TableRelated_ReturnsExpected(string tableName, string relatedTableName,
        string expected)
    {
        var result = tableName.CreateConstraintName(relatedTableName);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Table", "Related", "Field", "FK_Table_Relateds_Field")]
    [InlineData("Person", "Child", "Id", "FK_Person_Children_Id")]
    public void CreateConstraintName_TableRelatedField_ReturnsExpected(string tableName, string relatedTableName,
        string relatedFieldName, string expected)
    {
        var result = tableName.CreateConstraintName(relatedTableName, relatedFieldName);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Table", "Related", new[] { "Field1", "Field2" }, "FK_Table_Relateds_Field1_Field2")]
    [InlineData("Person", "Child", new[] { "Id", "Name" }, "FK_Person_Children_Id_Name")]
    [InlineData("Table", "Related", new string[] { }, "FK_Table_Relateds_")]
    public void CreateConstraintName_TableRelatedFields_ReturnsExpected(string tableName, string relatedTableName,
        string[] relatedFieldNames, string expected)
    {
        var result = tableName.CreateConstraintName(relatedTableName, relatedFieldNames);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Table", 1, "FK_Table_Table1")]
    [InlineData("Person", 2, "FK_Person_Person2")]
    public void CreateSelfRelatedConstraintName_ReturnsExpected(string tableName, int number, string expected)
    {
        var result = tableName.CreateSelfRelatedConstraintName(number);
        Assert.Equal(expected, result);
    }
}