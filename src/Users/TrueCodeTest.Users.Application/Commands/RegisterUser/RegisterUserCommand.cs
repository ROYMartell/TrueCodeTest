using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<Result<User>>;
