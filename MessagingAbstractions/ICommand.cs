using MediatR;
using OneOf;
using System.Collections.Generic;
using SystemToolsShared.Errors;

namespace MessagingAbstractions;

public interface ICommand : IRequest<OneOf<Unit, IEnumerable<Err>>>;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, IEnumerable<Err>>>;