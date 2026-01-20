using MediatR;
using OneOf;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, OneOf<TResponse, Err[]>>
    where TQuery : IQuery<TResponse>;