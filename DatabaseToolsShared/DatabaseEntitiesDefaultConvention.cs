using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DatabaseToolsShared;

//ამ კლასის დანიშნულებაა გააკეთოს საბოლოო ცვლილებები ბაზის ცხრილების მოდელების კონფიგურაციებში
//ეს ისეთი ცვლილებებია, რომლებიც ზოგადად ყველა მოდელისთვის მისაღებია. თუმცა გათვალისწინებულია გამონაკლისებიც
public class DatabaseEntitiesDefaultConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        //თითოეული ენტიტის ტიპისთვის
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            ////დავადგინოთ არის თუ არა უკვე მინიჭებული ცხრილის სახელი
            var tableNameAnnotation = entityType.GetTableName();
            ////თუ ცხრილის სახელი ცარიელია, მაშინ მივანიჭოთ ენტიტის ტიპის სახელის მრავლობითი ფორმა.
            ////ასევე ცხრილის სახელის შექქმნისას პირველი ასო დავაპატარავოთ
            if (string.IsNullOrEmpty(tableNameAnnotation))
                continue;
            //{
            //    tableNameAnnotation = entityType.ClrType.Name.Pluralize().UnCapitalize();
            //    entityType.SetTableName(tableNameAnnotation);
            //    Console.WriteLine("Entity table name changes to " + tableNameAnnotation);
            //}
            //else
            //{
            //    Console.WriteLine("Entity table has name " + tableNameAnnotation);
            //}

            //ველების სახელების დაყენება
            SetFieldNames(entityType);

            //ინდექსების სახელების დაყენება
            SetIndexesNames(entityType, tableNameAnnotation);

            //რეალციური კავშირების სახელების დაყენება
            SetRelationConstraintNames(entityType, tableNameAnnotation);
        }
    }

    private static void SetRelationConstraintNames(IConventionEntityType entityType, string tableNameAnnotation)
    {
        var foreignKeys = entityType.GetForeignKeys().ToList();
        //foreignKeys.Select(s=>s.PrincipalEntityType.GetTableName()).GroupBy(g=>g)

        var uniqueTableNamesWithMoreThenOneOccurence = foreignKeys.Select(s => s.PrincipalEntityType.GetTableName())
            .Where(w => w is not null).Cast<string>().GroupBy(s => s)
            .Select(g => new { Value = g.Key, Count = g.Count() }).Where(w => w.Count > 1).Select(s => s.Value)
            .ToList();

        var selfRelatedNumber = 0;
        //თითოეული გამოცხადებული კავშირისთვის
        foreach (var foreignKey in entityType.GetForeignKeys())
        {
            //დავადგინოთ არის თუ არა უკვე მინიჭებული კონსტრეინის სახელი
            var constraintNameAnnotation = foreignKey.GetConstraintName();
            //თუ უკვე მინიჭებულია, მაშინ ამ ინდექსს ვანებებთ თავს
            if (!string.IsNullOrEmpty(constraintNameAnnotation))
                continue;

            //დავადგინოთ ამ ტიპთან რელაციურ კავშირში მყოფი ცხრილის შესაბამისი ტიპი
            var relatedEntityType = foreignKey.PrincipalEntityType;

            //დავადგინოთ ამ ტიპის ცხრილის სახელი
            var relatedTableName = relatedEntityType.GetTableName();

            if (relatedTableName is null)
                continue;

            string constraintName;

            //თუ დაკავშირებული ცხრილი საკუთარი თავია, SelfRelated ყოფილა
            if (relatedEntityType.Name == entityType.Name)
            {
                constraintName = tableNameAnnotation.CreateSelfRelatedConstraintName(selfRelatedNumber);
                selfRelatedNumber++;
            }
            else if (uniqueTableNamesWithMoreThenOneOccurence.Contains(relatedTableName))
            {
                constraintName = tableNameAnnotation.CreateConstraintName(relatedTableName);
            }
            else
            {
                var relatedFieldName = foreignKey.Properties.Select(s => s.GetColumnName()).ToArray();
                constraintName = tableNameAnnotation.CreateConstraintName(relatedTableName, relatedFieldName);
            }

            foreignKey.SetConstraintName(constraintName);
        }
    }

    private static void SetFieldNames(IConventionEntityType entityType)
    {
        //ენტიტის თითოეული ველისთვის 
        foreach (var property in entityType.GetProperties())
        {
            ////ბაზის სვეტის სახელს მივანიჭოთ ველის სახელი პირველი ასოთი დაპატარავებულ ფორმაში
            //property.SetColumnName(property.Name.UnCapitalize());
            //თუ ველის ტიპი არის DateTime, მაშინ სვეტის ტიპი იყოს datetime
            if (property.ClrType == typeof(DateTime))
                property.SetColumnType("datetime");
            //თუ ველის ტიპი არის decimal, მაშინ სვეტის ტიპი იყოს money
            if (property.ClrType == typeof(decimal))
                property.SetColumnType("money");
        }
    }

    private static void SetIndexesNames(IConventionEntityType entityType, string tableNameAnnotation)
    {
        //ენტიტის თითოეული ინდექსისათვის
        foreach (var index in entityType.GetIndexes())
        {
            //დავადგინოთ არის თუ არა უკვე მინიჭებული ბაზის სახელი
            var indexNameAnnotation = index.GetDatabaseName();
            //თუ უკვე მინიჭებულია, მაშინ ამ ინდექსს ვანებებთ თავს
            if (!string.IsNullOrEmpty(indexNameAnnotation))
                continue;

            //დავადგინოთ ინდექსში შემავალი ველების სახელები
            var properties = index.Properties.Select(p => p.GetColumnName()).ToArray();

            //ნდექსის სახელი შევქმნათ ცხრილის სახელზე ველების სახელების დამატევით და უნიკალურობის გათვალისწინებით
            indexNameAnnotation = tableNameAnnotation.CreateIndexName(index.IsUnique, properties);

            //შექმნილი სახელი მივანიჭოთ ინდექსის ბაზის სახელს
            index.SetDatabaseName(indexNameAnnotation);
        }
    }
}