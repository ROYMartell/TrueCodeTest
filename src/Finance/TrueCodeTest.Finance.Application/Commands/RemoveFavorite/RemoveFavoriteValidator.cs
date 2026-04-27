using FluentValidation;

namespace TrueCodeTest.Finance.Application.Commands.RemoveFavorite;

public sealed class RemoveFavoriteValidator : AbstractValidator<RemoveFavoriteCommand>
{
    public RemoveFavoriteValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrencyId).GreaterThan(0);
    }
}
