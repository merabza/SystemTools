using MediatR;
using OneOf;
using SystemToolsShared.Errors;

namespace MediatRMessagingAbstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, OneOf<Unit, Err[]>> where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, Err[]>>
    where TCommand : ICommand<TResponse>;