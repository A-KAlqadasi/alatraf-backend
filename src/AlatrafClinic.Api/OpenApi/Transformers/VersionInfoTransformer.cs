using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace AlatrafClinic.Api.OpenApi.Transformers;

internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"AlatrafClinic API {version}";

        return Task.CompletedTask;
    }
}