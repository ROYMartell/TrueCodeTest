using FluentValidation;

namespace TrueCodeTest.Users.Application.Commands.Logout;

public sealed class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
