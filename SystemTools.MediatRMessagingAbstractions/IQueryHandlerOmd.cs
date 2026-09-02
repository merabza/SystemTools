using MediatR;
using SystemTools.SharedKernel;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQueryHandlerOmd<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQueryOmd<TResponse>;
