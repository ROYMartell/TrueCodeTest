using TrueCodeTest.Finance.Api.Endpoints;
using TrueCodeTest.Finance.Api.Extensions;
using TrueCodeTest.Finance.Application;
using TrueCodeTest.Finance.Infrastructure;
using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Shared.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFinanceApplication();
builder.Services.AddFinanceInfrastructure(builder.Configuration);
builder.Services.AddTrueCodeJwtAuth(builder.Configuration);
builder.Services.AddCurrentUser();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

builder.Services.AddFinanceSwagger();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();
app.MapFinanceEndpoints();

app.Run();

public partial class Program { }
