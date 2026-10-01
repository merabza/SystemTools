using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using SystemTools.Application.Abstractions.Messaging;
using SystemTools.SharedKernel;
using Xunit;

namespace SystemTools.Application.Abstractions.Tests;

public sealed class DependencyInjectionTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication(null, typeof(DependencyInjectionTests));
        return services.BuildServiceProvider();
    }

    private static void AssertScoped(IServiceCollection services, Type serviceType)
    {
        Assert.Contains(services, d => d.ServiceType == serviceType && d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddApplication_ReturnsTheSameCollection()
    {
        var services = new ServiceCollection();

        IServiceCollection returned = services.AddApplication(null, typeof(DependencyInjectionTests));

        Assert.Same(services, returned);
    }

    [Fact]
    public void AddApplication_RegistersTheHandlersOfTheGivenAssembliesAsScoped()
    {
        var services = new ServiceCollection();

        services.AddApplication(null, typeof(DependencyInjectionTests));

        AssertScoped(services, typeof(IQueryHandler<TestQuery, int>));
        AssertScoped(services, typeof(ICommandHandler<TestCommand>));
        AssertScoped(services, typeof(ICommandHandler<TestCommandWithResponse, int>));
    }

    [Fact]
    public void AddApplication_RegistersTheDomainEventHandlersOfTheGivenAssembliesAsScoped()
    {
        var services = new ServiceCollection();

        services.AddApplication(null, typeof(DependencyInjectionTests));

        Assert.Contains(services,
            d => d.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>) &&
                 d.ImplementationType == typeof(TestDomainEventHandler) && d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddApplication_RegistersNoHandlers_ForAnAssemblyWithoutHandlers()
    {
        var services = new ServiceCollection();

        services.AddApplication(null, typeof(Result));

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICommandHandler<TestCommand>));
    }

    [Fact]
    public async Task AddApplication_DecoratesTheCommandHandlerWithLoggingOverValidation()
    {
        await using ServiceProvider provider = BuildProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TestCommand>>();
        Result result = await handler.Handle(new TestCommand(), CancellationToken.None);

        Assert.Equal("LoggingDecorator", handler.GetType().DeclaringType?.Name);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, TestCommandHandler.Calls);
    }

    [Fact]
    public async Task AddApplication_DecoratesTheCommandWithResponseHandler()
    {
        await using ServiceProvider provider = BuildProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TestCommandWithResponse, int>>();
        Result<int> result = await handler.Handle(new TestCommandWithResponse(), CancellationToken.None);

        Assert.Equal("LoggingDecorator", handler.GetType().DeclaringType?.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task AddApplication_DecoratesTheQueryHandlerWithLogging()
    {
        await using ServiceProvider provider = BuildProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
        Result<int> result = await handler.Handle(new TestQuery(), CancellationToken.None);

        Assert.Equal("LoggingDecorator", handler.GetType().DeclaringType?.Name);
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public async Task AddApplication_ValidatesTheCommandBeforeTheHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication(null, typeof(DependencyInjectionTests));
        services.AddScoped<IValidator<TestCommandWithResponse>, FailingValidator>();
        await using ServiceProvider provider = services.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TestCommandWithResponse, int>>();
        Result<int> result = await handler.Handle(new TestCommandWithResponse(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);
    }

    [Fact]
    public async Task AddApplication_ValidatesTheCommandWithoutResponseBeforeTheHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication(null, typeof(DependencyInjectionTests));
        services.AddScoped<IValidator<ValidatedCommand>, FailingValidatedCommandValidator>();
        await using ServiceProvider provider = services.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ValidatedCommand>>();
        Result result = await handler.Handle(new ValidatedCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);
        Assert.Equal(0, ValidatedCommandHandler.Calls);
    }

    [Fact]
    public void AddApplication_LogsTheStartAndTheEnd()
    {
        var logger = new Mock<ILogger>();

        new ServiceCollection().AddApplication(logger.Object, typeof(DependencyInjectionTests));

        logger.Verify(l => l.Information("{MethodName} Started", "AddApplication"), Times.Once);
        logger.Verify(l => l.Information("{MethodName} Finished", "AddApplication"), Times.Once);
    }

    public sealed record TestQuery : IQuery<int>;

    public sealed record TestCommand : ICommand;

    public sealed record TestCommandWithResponse : ICommand<int>;

    public sealed record TestDomainEvent : IDomainEvent;

    //internal: AddApplication must register non-public handlers too
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by dependency injection")]
    internal sealed class TestQueryHandler : IQueryHandler<TestQuery, int>
    {
        public Task<Result<int>> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(7));
        }
    }

    //internal: AddApplication must register non-public handlers too
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by dependency injection")]
    internal sealed class TestCommandHandler : ICommandHandler<TestCommand>
    {
        private static int _calls;

        public static int Calls => _calls;

        public Task<Result> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _calls);
            return Task.FromResult(Result.Success());
        }
    }

    //internal: AddApplication must register non-public handlers too
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by dependency injection")]
    internal sealed class TestCommandWithResponseHandler : ICommandHandler<TestCommandWithResponse, int>
    {
        public Task<Result<int>> Handle(TestCommandWithResponse command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(42));
        }
    }

    public sealed record ValidatedCommand : ICommand;

    public sealed class ValidatedCommandHandler : ICommandHandler<ValidatedCommand>
    {
        private static int _calls;

        public static int Calls => _calls;

        public Task<Result> Handle(ValidatedCommand command, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _calls);
            return Task.FromResult(Result.Success());
        }
    }

    //Registered by the tests themselves: AddApplication registers only the validators of SystemTools
    public sealed class FailingValidator : AbstractValidator<TestCommandWithResponse>
    {
        public FailingValidator()
        {
            RuleFor(x => x).Must(_ => false).WithErrorCode("Invalid");
        }
    }

    public sealed class FailingValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        public FailingValidatedCommandValidator()
        {
            RuleFor(x => x).Must(_ => false).WithErrorCode("Invalid");
        }
    }

    //internal: AddApplication must register non-public handlers too
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by dependency injection")]
    internal sealed class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public Task Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
