using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspireSample.AppHost.Extensions;

public static class ResourceBuilderExtensions
{
    private static IResourceBuilder<T> WithOpenApiDocs<T, TK>(this IResourceBuilder<T> builder,
        TK openApiOption)
        where T : IResourceWithEndpoints
        where TK : OpenApiOption
    {
        return builder.WithCommand(
            openApiOption.Name,
            openApiOption.DisplayName,
             _ =>
            {
                try
                {
                    var endpoint = builder.GetEndpoint("https");
                    var url = $"{endpoint.Url}/{openApiOption.OpenApiUiPath}";
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
        builder.WithOpenApiDocs(new SwaggerUiOption());
        return builder;
    }

    public static IResourceBuilder<T> WithRedoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        builder.WithOpenApiDocs(new RedocOption());
        return builder;
    }

    public static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        builder.WithOpenApiDocs(new ScalarOption());
        return builder;
    }
}

public abstract class OpenApiOption
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string OpenApiUiPath { get; set; } = default!;
}

public class SwaggerUiOption : OpenApiOption
{
    public SwaggerUiOption()
    {
        Name = "swagger-ui-docs";
        DisplayName = "Swagger API Documentation";
        OpenApiUiPath = "swagger";
    }
}

public class RedocOption : OpenApiOption
{
    public RedocOption()
    {
        Name = "redoc-docs";
        DisplayName = "Redoc API Documentation";
        OpenApiUiPath = "api-docs";
    }
}

public class ScalarOption : OpenApiOption
{
    public ScalarOption()
    {
        Name = "scalar-docs";
        DisplayName = "Scalar API Documentation";
        OpenApiUiPath = "scalar";
    }
}