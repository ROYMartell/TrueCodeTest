using MediatR;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Users.Application.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
