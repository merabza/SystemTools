using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQueryHandlerOmd<in TQuery, TResponse> : IRequestHandler<TQuery, OneOf<TResponse, ErrorOmd[]>>
    where TQuery : IQueryOmd<TResponse>;
