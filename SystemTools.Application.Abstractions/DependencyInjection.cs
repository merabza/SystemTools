using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SystemTools.Application.Abstractions.Behaviors;
using SystemTools.Application.Abstractions.Messaging;
using SystemTools.SharedKernel;

namespace SystemTools.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, ILogger? debugLogger,
        params Type[] types)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddApplication));

        services.Scan(scan => scan.FromAssembliesOf(types)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), false).AsImplementedInterfaces()
            .WithScopedLifetime().AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), false)
            .AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), false).AsImplementedInterfaces()
            .WithScopedLifetime());

        //TryDecorate, რომ იმ სოლუშენებში, სადაც რომელიმე სახეობის ჰენდლერი საერთოდ არ არის
        //რეგისტრირებული, Decorate-მა DecorationException არ ისროლოს
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), false).AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        debugLogger?.Information("{MethodName} Finished", nameof(AddApplication));

        return services;
    }
}
