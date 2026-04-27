using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Users.Application.Abstractions;

namespace TrueCodeTest.Users.Application.Commands.Login;

public sealed record LoginCommand(string Name, string Password) : IRequest<Result<TokenPair>>;
