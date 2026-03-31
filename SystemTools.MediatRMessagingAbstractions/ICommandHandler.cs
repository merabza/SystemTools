using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, OneOf<Unit, Error[]>>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, Error[]>>
    where TCommand : ICommand<TResponse>;
