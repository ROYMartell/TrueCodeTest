using TrueCodeTest.Shared.Web;

namespace TrueCodeTest.Finance.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddFinanceSwagger(this IServiceCollection services) =>
        services.AddTrueCodeBearerSwagger(
            "TrueCodeTest.Finance",
            "Currency rates & user favorites");
}
