using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SystemTools.DatabaseToolsShared;

//ეს არის ზოგადი კლასი მიგრაციასთან სამუშაოდ
//თუმცა მე გადავაკეთე ისე, რომ კონსტრუქტორი ღებულობს ინფორმაციას:
//1. სად ეძებოს პარამეტრების ფაილი, ==\== ამის გადაცემა გადავიფიქრე, რადგან კოდი მიმდინარე ფოლდერში იყურება
//და უფრო სწორია, თუ მიგრაციის პროცესს იმ ფოლდერიდან გავუშვებთ, რომელშიც პარამეტრებია ჩაწერილი.
//მიგრაციის და ბაზის კონტექსტის შესახებ ინფორმაციის გადაწოდება კი მიგრაციის ბრძანებისათვის შესაძლებელია და სკრიპტიდან მოხდება
//2. რა ჰქვია პარამეტრების ფაილს
//3. რომელ პარამეტრში წერია ბაზასთან დასაკავშირებელი სტრიქონი
//ბაზასთან დაკავშირების სტრიქონის გადმოწოდება არასწორია, რადგან მომიწევდა ამ სტრიქონის გამშვები პროექტის კოდში ჩაშენება.
//რაც უსაფრთხოების თვალსაზრისით არასწორია
public abstract class SqlServerDesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
{
    private readonly string _assemblyName;
    private readonly string _connectionParamName;
    private readonly string? _parametersJsonFileName;
    private readonly bool _useAppSettingsFromAssemblyFolder;

    protected SqlServerDesignTimeDbContextFactory(string assemblyName, string connectionParamName,
        string? parametersJsonFileName = null)
    {
        _assemblyName = assemblyName;
        _connectionParamName = connectionParamName;
        _parametersJsonFileName = parametersJsonFileName;
        Console.WriteLine($"DesignTimeDbContextFactory assemblyName = {assemblyName}");
        Console.WriteLine($"DesignTimeDbContextFactory connectionParamName = {connectionParamName}");
        Console.WriteLine($"DesignTimeDbContextFactory parametersJsonFileName = {parametersJsonFileName ?? "null"}");
    }

    //ეს კონსტრუქტორი გამოიყენება მაშინ, როცა პარამეტრები უნდა წამოვიდეს გამშვები პროექტის appsettings.json ფაილიდან.
    //appsettings.json იძებნება იმ ფოლდერში, სადაც მემკვიდრე კლასის შემცველი ასემბლი დევს,
    //ამიტომ შედეგი არ არის დამოკიდებული იმაზე, თუ რომელი ფოლდერიდან გაეშვება მიგრაციის ბრძანება
    protected SqlServerDesignTimeDbContextFactory(string assemblyName, string connectionParamName,
        bool useAppSettingsFromAssemblyFolder) : this(assemblyName, connectionParamName)
    {
        _useAppSettingsFromAssemblyFolder = useAppSettingsFromAssemblyFolder;
    }

    public T CreateDbContext(string[] args)
    {
        Console.WriteLine($"Pass 1... CurrentDirectory is {Directory.GetCurrentDirectory()}");

        //თუ მითითებულია ასემბლის ფოლდერიდან წაკითხვა, ვიყენებთ იმ ფოლდერს, სადაც მემკვიდრე კლასის ასემბლი დევს
        string basePath = _useAppSettingsFromAssemblyFolder
            ? Path.GetDirectoryName(GetType().Assembly.Location) ?? Directory.GetCurrentDirectory()
            : Directory.GetCurrentDirectory();

        Console.WriteLine($"Pass 2... basePath is {basePath}");

        //თუ პარამეტრების json ფაილის სახელი პირდაპირ არ არის გადმოცემული, ვიყენებთ სტანდარტულ სახელს appsettings.json
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(basePath)
            .AddJsonFile(_parametersJsonFileName ?? "appsettings.json", false, true);
        //.AddEncryptedJsonFile(Path.Combine(pathToContentRoot, "appsettingsEncoded.json"), optional: false, reloadOnChange: true, Key,
        //  Path.Combine(pathToContentRoot, "appsetenkeys.json"))

        //საიდუმლო პარამეტრები (მაგალითად ბაზასთან დასაკავშირებელი სტრიქონი) კოდში და appsettings.json-ში არ ინახება.
        //ისინი მოდის User Secrets-იდან (dotnet user-secrets set), რომლის იდენტიფიკატორი (UserSecretsId) წერია
        //მემკვიდრე კლასის შემცველ პროექტში. User Secrets გადაფარავს appsettings.json-ის მნიშვნელობებს
        if (_useAppSettingsFromAssemblyFolder)
        {
            configurationBuilder.AddUserSecrets(GetType().Assembly, true);
        }

        IConfigurationRoot configuration = configurationBuilder.Build();
        Console.WriteLine("Pass 3...");
        string? connectionString = configuration[_connectionParamName];
        Console.WriteLine("Pass 4...");

        var builder = new DbContextOptionsBuilder<T>();
        Console.WriteLine("Pass 5...");
        builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(_assemblyName));
        Console.WriteLine("Pass 6...");
        return CreateDbContext(builder.Options);
    }

    //მემკვიდრე კლასი ქმნის კონტექსტის ობიექტს პირდაპირ, რეფლექსიის გარეშე
    protected abstract T CreateDbContext(DbContextOptions<T> options);
}
