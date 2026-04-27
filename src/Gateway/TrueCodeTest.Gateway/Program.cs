using TrueCodeTest.Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTrueCodeJwtAuth(builder.Configuration);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var swaggerSources = builder.Configuration
    .GetSection("SwaggerSources")
    .Get<Dictionary<string, string>>() ?? new();

app.UseSwaggerUI(options =>
{
    foreach (var (name, url) in swaggerSources)
        options.SwaggerEndpoint(url, name);
    options.RoutePrefix = "swagger";
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();
app.MapReverseProxy();

app.Run();

public partial class Program { }
