using MediatR;
using OneOf;
using SystemToolsShared.Errors;

namespace MediatRMessagingAbstractions;

public interface ICommand : IRequest<OneOf<Unit, Err[]>>;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, Err[]>>;