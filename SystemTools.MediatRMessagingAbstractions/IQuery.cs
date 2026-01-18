using MediatR;
using OneOf;
using SystemToolsShared.Errors;

namespace MediatRMessagingAbstractions;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, Err[]>>;