using MediatR;
using SystemTools.SharedKernel;

namespace SystemTools.MediatRMessagingAbstractions;

public interface ICommandOmd : IRequest<Result>;

public interface ICommandOmd<TResponse> : IRequest<Result<TResponse>>;
