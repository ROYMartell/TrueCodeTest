using FluentValidation;

namespace TrueCodeTest.Users.Application.Commands.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
