using System.Collections.Generic;
using MediatR;
using OneOf;
using SystemToolsShared;

namespace MessagingAbstractions;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, IEnumerable<Err>>>
{
}