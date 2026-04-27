using FluentValidation;

namespace TrueCodeTest.Users.Application.Commands.Refresh;

public sealed class RefreshValidator : AbstractValidator<RefreshCommand>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
