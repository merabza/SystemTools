using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommand : IRequest<OneOf<Unit, ErrorOmd[]>>;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, ErrorOmd[]>>;
