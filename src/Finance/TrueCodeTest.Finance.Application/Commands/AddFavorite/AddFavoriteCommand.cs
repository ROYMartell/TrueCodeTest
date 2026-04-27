using MediatR;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Commands.AddFavorite;

public sealed record AddFavoriteCommand(Guid UserId, int CurrencyId) : IRequest<Result>;
