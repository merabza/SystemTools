using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommandHandlerOmd<in TCommand> : IRequestHandler<TCommand, OneOf<Unit, ErrorOmd[]>>
    where TCommand : ICommandOmd;

public interface ICommandHandlerOmd<in TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, ErrorOmd[]>>
    where TCommand : ICommandOmd<TResponse>;
