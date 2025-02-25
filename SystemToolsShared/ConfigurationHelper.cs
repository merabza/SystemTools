using System.Text;

namespace SystemToolsShared;

public static class ConfigurationHelper
{
    private const string ForeignKeyPrefix = "FK_";
    private const string IndexPrefix = "IX_";
    public const string ColumnTypeNText = "ntext";
    private const string UniqueText = "_Unique";

    public static string CreateIndexName(this string tableName, bool unique, params string[] fieldNames)
    {
        var sb = new StringBuilder(IndexPrefix);
        sb.Append(tableName);
        foreach (var fieldName in fieldNames)
        {
            sb.Append('_');
            sb.Append(fieldName.UnCapitalize());
        }

        if (unique)
            sb.Append(UniqueText);
        var indexName = sb.ToString();
        return indexName.Length > 128 ? indexName[..128] : indexName;
    }

    public static string CreateConstraintName(this string tableName, string relatedTableName)
    {
        return $"{ForeignKeyPrefix}{tableName}_{relatedTableName.Pluralize()}";
    }

    public static string CreateConstraintName(this string tableName, string relatedTableName, string relatedFieldName)
    {
        return $"{ForeignKeyPrefix}{tableName}_{relatedTableName.Pluralize()}_{relatedFieldName}";
    }

    public static string CreateConstraintName(this string tableName, string relatedTableName,
        string[] relatedFieldNames)
    {
        var fieldNamesInRow = relatedFieldNames.Length > 0 ? string.Join('_', relatedFieldNames) : string.Empty;
        return
            $"{ForeignKeyPrefix}{tableName}_{relatedTableName.Pluralize()}_{fieldNamesInRow}";
    }

    public static string CreateSelfRelatedConstraintName(this string tableName, int number)
    {
        return $"{ForeignKeyPrefix}{tableName}_{tableName}{number}";
    }
}