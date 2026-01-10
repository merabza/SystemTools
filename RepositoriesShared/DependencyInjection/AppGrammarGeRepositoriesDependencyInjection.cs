using DomainShared;
using DomainShared.Repositories;
using Microsoft.Extensions.DependencyInjection;

//using RepositoriesAbstraction;

namespace RepositoriesShared.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class RepositoriesSharedDependencyInjection
{
    public static IServiceCollection AddAppGrammarGeRepositories(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddAppGrammarGeRepositories)} Started");

        //services.AddScoped<IMasterDataRepository, AgrMasterDataRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        if (debugMode)
            Console.WriteLine($"{nameof(AddAppGrammarGeRepositories)} Finished");

        return services;
    }
}