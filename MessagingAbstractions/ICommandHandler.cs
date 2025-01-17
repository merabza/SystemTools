using MediatR;
using OneOf;
using System.Collections.Generic;
using SystemToolsShared.Errors;

namespace MessagingAbstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, OneOf<Unit, IEnumerable<Err>>>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, IEnumerable<Err>>>
    where TCommand : ICommand<TResponse>;