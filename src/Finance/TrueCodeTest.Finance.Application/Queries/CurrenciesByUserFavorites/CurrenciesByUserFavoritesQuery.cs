using MediatR;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Queries.CurrenciesByUserFavorites;

/// <summary>Курсы валют по избранному пользователя (favorites = источник id).</summary>
public sealed record CurrenciesByUserFavoritesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<Currency>>>;
