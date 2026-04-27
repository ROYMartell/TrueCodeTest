using MediatR;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Application.Errors;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Shared.Kernel.Abstractions;

namespace TrueCodeTest.Finance.Application.Commands.AddFavorite;

public sealed class AddFavoriteHandler(
    IUserFavoriteRepository favorites,
    ICurrencyRepository currencies,
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<AddFavoriteCommand, Result>
{
    public async Task<Result> Handle(AddFavoriteCommand request, CancellationToken ct)
    {
        if (!await currencies.ExistsAsync(request.CurrencyId, ct))
            return FinanceErrors.CurrencyNotFound;

        var existing = await favorites.FindAsync(request.UserId, request.CurrencyId, ct);
        if (existing is not null)
            return FinanceErrors.FavoriteAlreadyExists;

        await favorites.AddAsync(new UserFavorite
        {
            UserId = request.UserId,
            CurrencyId = request.CurrencyId,
            CreatedAt = clock.UtcNow,
        }, ct);

        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
