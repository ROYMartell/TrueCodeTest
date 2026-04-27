using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TrueCodeTest.Shared.Web;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddTrueCodeBearerSwagger(
        this IServiceCollection services,
        string documentTitle,
        string? documentDescription = null,
        string documentVersion = "v1")
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(documentVersion, new OpenApiInfo
            {
                Title = documentTitle,
                Version = documentVersion,
                Description = documentDescription,
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            };

            options.AddSecurityDefinition("Bearer", scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [scheme] = Array.Empty<string>(),
            });
        });

        return services;
    }
}
