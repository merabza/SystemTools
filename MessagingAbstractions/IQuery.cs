using MediatR;
using OneOf;
using System.Collections.Generic;
using SystemToolsShared.Errors;

namespace MessagingAbstractions;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, IEnumerable<Err>>>;