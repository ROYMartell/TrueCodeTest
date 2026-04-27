using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Commands.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<RegisterUserCommand, Result<User>>
{
    public async Task<Result<User>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await users.ExistsByNameAsync(request.Name, ct))
            return UserErrors.NameTaken;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PasswordHash = hasher.Hash(request.Password),
            CreatedAt = clock.UtcNow,
        };

        await users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        return user;
    }
}
