using MediatR;
using TrueCodeTest.Finance.Application.Commands.AddFavorite;
using TrueCodeTest.Finance.Application.Commands.RemoveFavorite;
using TrueCodeTest.Finance.Application.Queries.CurrenciesByUserFavorites;
using TrueCodeTest.Finance.Application.Queries.ListCurrencies;
using TrueCodeTest.Finance.Contracts.Currencies;
using TrueCodeTest.Finance.Mappers;
using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Shared.Web;

namespace TrueCodeTest.Finance.Api.Endpoints;

public static class FinanceEndpoints
{
    public static IEndpointRouteBuilder MapFinanceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/finance")
            .WithTags("Finance")
            .RequireAuthorization();

        group.MapGet("/currencies", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ListCurrenciesQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value.ToDto())
                    : result.Error.ToProblem();
            })
            .WithName("ListCurrencies")
            .Produces<IReadOnlyList<CurrencyDto>>();

        group.MapGet("/favorites", async (ICurrentUser currentUser, ISender sender, CancellationToken ct) =>
            {
                if (currentUser.UserId is null)
                    return Results.Unauthorized();

                var result = await sender.Send(new CurrenciesByUserFavoritesQuery(currentUser.UserId.Value), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value.ToDto())
                    : result.Error.ToProblem();
            })
            .WithName("GetUserFavorites")
            .Produces<IReadOnlyList<CurrencyDto>>();

        group.MapPost("/favorites/{currencyId:int}", async (
                int currencyId,
                ICurrentUser currentUser,
                ISender sender,
                CancellationToken ct) =>
            {
                if (currentUser.UserId is null)
                    return Results.Unauthorized();

                var result = await sender.Send(new AddFavoriteCommand(currentUser.UserId.Value, currencyId), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error.ToProblem();
            })
            .WithName("AddFavorite")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/favorites/{currencyId:int}", async (
                int currencyId,
                ICurrentUser currentUser,
                ISender sender,
                CancellationToken ct) =>
            {
                if (currentUser.UserId is null)
                    return Results.Unauthorized();

                var result = await sender.Send(new RemoveFavoriteCommand(currentUser.UserId.Value, currencyId), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error.ToProblem();
            })
            .WithName("RemoveFavorite")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
