using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommandOmd : IRequest<OneOf<Unit, ErrorOmd[]>>;

public interface ICommandOmd<TResponse> : IRequest<OneOf<TResponse, ErrorOmd[]>>;
