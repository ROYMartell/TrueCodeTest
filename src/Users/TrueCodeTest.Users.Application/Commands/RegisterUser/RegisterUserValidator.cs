using FluentValidation;

namespace TrueCodeTest.Users.Application.Commands.RegisterUser;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches("^[a-zA-Z0-9_.-]+$")
            .WithMessage("Name can contain letters, digits, underscore, dot and hyphen only");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(128);
    }
}
