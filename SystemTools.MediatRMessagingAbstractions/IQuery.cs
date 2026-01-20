using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, Err[]>>;
