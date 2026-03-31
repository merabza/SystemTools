using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommand : IRequest<OneOf<Unit, Error[]>>;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, Error[]>>;
