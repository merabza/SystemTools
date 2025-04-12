using System.Collections.Generic;
using MediatR;
using OneOf;
using SystemToolsShared.Errors;

namespace MediatRMessagingAbstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, OneOf<TResponse, IEnumerable<Err>>>
    where TQuery : IQuery<TResponse>;