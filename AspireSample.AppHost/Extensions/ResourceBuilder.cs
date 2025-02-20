using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspireSample.AppHost.Extensions;

public static class ResourceBuilderExtensions
{
    private static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder,
        string name, string displayName, string openApiUiPath)
        where T : IResourceWithEndpoints
    {
        return builder.WithCommand(
            name,
            displayName,
            _ =>
            {
                try
                {
                    var endpoint = builder.GetEndpoint("https");
                    var url = $"{endpoint.Url}/{openApiUiPath}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

                    return Task.FromResult(new ExecuteCommandResult { Success = true });
                }
                catch (Exception e)
                {
                    return Task.FromResult(new ExecuteCommandResult
                    {
                        Success = false,
                        ErrorMessage = e.Message
                    });
                }
            },
            context => context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled,
            iconName: "Document",
            iconVariant: IconVariant.Filled
        );
    }

    public static IResourceBuilder<T> WithSwaggerUi<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        builder.WithOpenApiDocs("swagger-ui-docs", "Swagger API Documentation", "swagger");
        return builder;
    }

    public static IResourceBuilder<T> WithRedoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        builder.WithOpenApiDocs("redoc-docs", "Redoc API Documentation", "api-docs");
        return builder;
    }

    public static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        builder.WithOpenApiDocs("scalar-docs", "Scalar API Documentation", "scalar");
        return builder;
    }
}

public abstract class OpenApiOption
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string openApiUiPath { get; set; } = default!;
}

public class SwaggerUiOption : OpenApiOption
{
    public SwaggerUiOption()
    {
        Name = "swagger-ui-docs";
        DisplayName = "Swagger API Documentation";
        openApiUiPath = "swagger";
    }
}


public class RedocOption : OpenApiOption
{
    public RedocOption()
    {
        Name = "redoc-docs";
        DisplayName = "Redoc API Documentation";
        openApiUiPath = "api-docs";
    }
}


public class ScalarOption : OpenApiOption
{
    public ScalarOption()
    {
        Name = "scalar-docs";
        DisplayName = "Scalar API Documentation";
        openApiUiPath = "scalar";
    }
}