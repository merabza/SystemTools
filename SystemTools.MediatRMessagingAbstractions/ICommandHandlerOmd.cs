using MediatR;
using SystemTools.SharedKernel;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommandHandlerOmd<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommandOmd;

public interface ICommandHandlerOmd<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommandOmd<TResponse>;
