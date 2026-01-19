using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace SystemTools.DatabaseToolsShared;

//ამ კლასის დანიშნულებაა გააკეთოს საბოლოო ცვლილებები ბაზის ცხრილების მოდელების კონფიგურაციებში
//ეს ისეთი ცვლილებებია, რომლებიც ზოგადად ყველა მოდელისთვის მისაღებია. თუმცა გათვალისწინებულია გამონაკლისებიც
public sealed class DatabaseEntitiesDefaultConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        //თითოეული ენტიტის ტიპისთვის
        foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            ////დავადგინოთ არის თუ არა უკვე მინიჭებული ცხრილის სახელი
            string? tableNameAnnotation = entityType.GetTableName();
            ////თუ ცხრილის სახელი ცარიელია, მაშინ მივანიჭოთ ენტიტის ტიპის სახელის მრავლობითი ფორმა.
            ////ასევე ცხრილის სახელის შექქმნისას პირველი ასო დავაპატარავოთ
            if (string.IsNullOrEmpty(tableNameAnnotation))
            {
                continue;
            }
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
        List<IConventionForeignKey> foreignKeys = entityType.GetForeignKeys().ToList();
        //foreignKeys.Select(s=>s.PrincipalEntityType.GetTableName()).GroupBy(g=>g)

        List<string> uniqueTableNamesWithMoreThenOneOccurence = foreignKeys
            .Select(s => s.PrincipalEntityType.GetTableName()).Where(w => w is not null).Cast<string>().GroupBy(s => s)
            .Select(g => new { Value = g.Key, Count = g.Count() }).Where(w => w.Count > 1).Select(s => s.Value)
            .ToList();

        int selfRelatedNumber = 0;
        //თითოეული გამოცხადებული კავშირისთვის
        foreach (IConventionForeignKey foreignKey in entityType.GetForeignKeys())
        {
            //დავადგინოთ არის თუ არა უკვე მინიჭებული კონსტრეინის სახელი
            string? constraintNameAnnotation = foreignKey.GetConstraintName();
            //თუ უკვე მინიჭებულია, მაშინ ამ ინდექსს ვანებებთ თავს
            if (!string.IsNullOrEmpty(constraintNameAnnotation))
            {
                continue;
            }

            //დავადგინოთ ამ ტიპთან რელაციურ კავშირში მყოფი ცხრილის შესაბამისი ტიპი
            IConventionEntityType relatedEntityType = foreignKey.PrincipalEntityType;

            //დავადგინოთ ამ ტიპის ცხრილის სახელი
            string? relatedTableName = relatedEntityType.GetTableName();

            if (relatedTableName is null)
            {
                continue;
            }

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
                string[] relatedFieldName = foreignKey.Properties.Select(s => s.GetColumnName()).ToArray();
                constraintName = tableNameAnnotation.CreateConstraintName(relatedTableName, relatedFieldName);
            }

            foreignKey.SetConstraintName(constraintName);
        }
    }

    private static void SetFieldNames(IConventionEntityType entityType)
    {
        //ენტიტის თითოეული ველისთვის 
        foreach (IConventionProperty property in entityType.GetProperties())
        {
            ////ბაზის სვეტის სახელს მივანიჭოთ ველის სახელი პირველი ასოთი დაპატარავებულ ფორმაში
            //property.SetColumnName(property.Name.UnCapitalize());
            //თუ ველის ტიპი არის DateTime, მაშინ სვეტის ტიპი იყოს datetime

            Type clrType = property.ClrType;

            bool isNullable = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (isNullable)
            {
                clrType = clrType.GetGenericArguments()[0];
            }

            if (clrType == typeof(DateTime))
            {
                property.SetColumnType("datetime");
            }

            //თუ ველის ტიპი არის decimal, მაშინ სვეტის ტიპი იყოს money
            if (clrType == typeof(decimal))
            {
                property.SetColumnType("money");
            }
        }
    }

    private static void SetIndexesNames(IConventionEntityType entityType, string tableNameAnnotation)
    {
        //ენტიტის თითოეული ინდექსისათვის
        foreach (IConventionIndex index in entityType.GetIndexes())
        {
            //დავადგინოთ არის თუ არა უკვე მინიჭებული ბაზის სახელი
            string? indexNameAnnotation = index.GetDatabaseName();
            //თუ უკვე მინიჭებულია, მაშინ ამ ინდექსს ვანებებთ თავს
            if (!string.IsNullOrEmpty(indexNameAnnotation))
            {
                continue;
            }

            //დავადგინოთ ინდექსში შემავალი ველების სახელები
            string[] properties = index.Properties.Select(p => p.GetColumnName()).ToArray();

            //ნდექსის სახელი შევქმნათ ცხრილის სახელზე ველების სახელების დამატევით და უნიკალურობის გათვალისწინებით
            indexNameAnnotation = tableNameAnnotation.CreateIndexName(index.IsUnique, properties);

            //შექმნილი სახელი მივანიჭოთ ინდექსის ბაზის სახელს
            index.SetDatabaseName(indexNameAnnotation);
        }
    }
}
