using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQueryOmd<TResponse> : IRequest<OneOf<TResponse, ErrorOmd[]>>;
