using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrueCodeTest.Shared.Web;
using TrueCodeTest.Users.Application.Commands.Login;
using TrueCodeTest.Users.Application.Commands.Logout;
using TrueCodeTest.Users.Application.Commands.Refresh;
using TrueCodeTest.Users.Application.Commands.RegisterUser;
using TrueCodeTest.Users.Contracts.Auth;
using TrueCodeTest.Users.Mappers;

namespace TrueCodeTest.Users.Api.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/register", async (
                [FromBody] RegisterRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new RegisterUserCommand(request.Name, request.Password), ct);
                return result.IsSuccess
                    ? Results.Created($"/api/users/{result.Value.Id}", result.Value.ToDto())
                    : result.Error.ToProblem();
            })
            .WithName("RegisterUser")
            .AllowAnonymous()
            .Produces<UserDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();

        group.MapPost("/login", async (
                [FromBody] LoginRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new LoginCommand(request.Name, request.Password), ct);
                return result.IsSuccess
                    ? Results.Ok(ToAuthResponse(result.Value))
                    : result.Error.ToProblem();
            })
            .WithName("LoginUser")
            .AllowAnonymous()
            .Produces<AuthResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", async (
                [FromBody] RefreshRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new RefreshCommand(request.RefreshToken), ct);
                return result.IsSuccess
                    ? Results.Ok(ToAuthResponse(result.Value))
                    : result.Error.ToProblem();
            })
            .WithName("RefreshToken")
            .AllowAnonymous()
            .Produces<AuthResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", async (
                [FromBody] LogoutRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new LogoutCommand(request.RefreshToken), ct);
                return result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();
            })
            .WithName("LogoutUser")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    private static AuthResponse ToAuthResponse(Application.Abstractions.TokenPair pair) =>
        new(pair.Access.Token, pair.Refresh.Token, pair.Access.ExpiresAt, pair.Refresh.ExpiresAt);
}
