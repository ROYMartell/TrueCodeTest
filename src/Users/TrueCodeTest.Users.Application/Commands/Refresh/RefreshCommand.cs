using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Users.Application.Abstractions;

namespace TrueCodeTest.Users.Application.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken) : IRequest<Result<TokenPair>>;
