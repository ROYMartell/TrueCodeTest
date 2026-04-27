using TrueCodeTest.Shared.Web;

namespace TrueCodeTest.Users.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddUsersSwagger(this IServiceCollection services) =>
        services.AddTrueCodeBearerSwagger(
            "TrueCodeTest.Users",
            "User management & authentication service");
}
