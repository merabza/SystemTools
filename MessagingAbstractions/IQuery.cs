using System.Collections.Generic;
using MediatR;
using OneOf;
using SystemToolsShared.Errors;

namespace MessagingAbstractions;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, IEnumerable<Err>>>;