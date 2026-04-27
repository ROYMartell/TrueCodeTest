using FluentValidation;

namespace TrueCodeTest.Finance.Application.Commands.AddFavorite;

public sealed class AddFavoriteValidator : AbstractValidator<AddFavoriteCommand>
{
    public AddFavoriteValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrencyId).GreaterThan(0);
    }
}
