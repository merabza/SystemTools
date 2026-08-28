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
        if (debugLogger is not null)
        {
            debugLogger.Information("{MethodName} Started", nameof(AddApplication));
        }
        else
        {
            return services;
        }

        services.Scan(scan => scan.FromAssembliesOf(types)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), false).AsImplementedInterfaces()
            .WithScopedLifetime().AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), false)
            .AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), false).AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), false).AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        debugLogger.Information("{MethodName} Finished", nameof(AddApplication));

        return services;
    }
}
