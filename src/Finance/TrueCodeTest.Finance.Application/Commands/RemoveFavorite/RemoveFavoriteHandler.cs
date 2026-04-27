using MediatR;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Application.Errors;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Commands.RemoveFavorite;

public sealed class RemoveFavoriteHandler(IUserFavoriteRepository favorites, IUnitOfWork uow)
    : IRequestHandler<RemoveFavoriteCommand, Result>
{
    public async Task<Result> Handle(RemoveFavoriteCommand request, CancellationToken ct)
    {
        var existing = await favorites.FindAsync(request.UserId, request.CurrencyId, ct);
        if (existing is null)
            return FinanceErrors.FavoriteNotFound;

        favorites.Remove(existing);
        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
