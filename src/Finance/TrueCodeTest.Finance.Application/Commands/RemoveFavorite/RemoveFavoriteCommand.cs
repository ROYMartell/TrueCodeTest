using MediatR;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Commands.RemoveFavorite;

public sealed record RemoveFavoriteCommand(Guid UserId, int CurrencyId) : IRequest<Result>;
