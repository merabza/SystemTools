using MediatR;
using SystemTools.SharedKernel;

namespace SystemTools.MediatRMessagingAbstractions;

public interface IQueryOmd<TResponse> : IRequest<Result<TResponse>>;
