using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Shared.Web;
using TrueCodeTest.Users.Api.Endpoints;
using TrueCodeTest.Users.Api.Extensions;
using TrueCodeTest.Users.Application;
using TrueCodeTest.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddTrueCodeJwtAuth(builder.Configuration);
builder.Services.AddCurrentUser();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

builder.Services.AddUsersSwagger();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();
app.MapUsersEndpoints();

app.Run();

public partial class Program { }
