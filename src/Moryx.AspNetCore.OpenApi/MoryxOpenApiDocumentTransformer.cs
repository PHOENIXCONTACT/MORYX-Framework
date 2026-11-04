// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

/// <summary>
/// Document transformer to improve our API documentation
/// </summary>
internal class MoryxOpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        SortPathEntries(document);

        EnsureSseItemBaseExists(document);
    }

    /// <summary>
    /// Sort path entries by tag
    /// </summary>
    /// <param name="document"></param>
    private static void SortPathEntries(OpenApiDocument document)
    {
        var path = new OpenApiPaths();
        var sortLookup = new Dictionary<string, int>();
        document.Tags = document.Tags.OrderBy(t => t.Name).ToHashSet();

        var sorted = document.Paths.OrderBy((p) =>
        {
            if (p.Value.Operations.Count == 0)
            {
                return "";
            }

            var op = p.Value.Operations.First().Value;
            return op.Tags.FirstOrDefault()?.Name ?? "";
        });

        foreach (var item in sorted)
        {
            path.Add(item.Key, item.Value);
        }

        document.Paths = path;
    }

    private static void EnsureSseItemBaseExists(OpenApiDocument document)
    {
        var sseItemBaseSchema = new OpenApiSchema()
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                { "type", new OpenApiSchema() { Type = JsonSchemaType.String } },
                { "eventId",  new OpenApiSchema() { Type = JsonSchemaType.String } },
                { "reconnectInterval", new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String,
                        Pattern = "^-?(\\d+\\.)?\\d{2}:\\d{2}:\\d{2}(\\.\\d{1,7})?$",
                    }
                }
            }
        };
        document.AddComponent(MoryxSseOperationTransformer.SseItemBaseSchemaId, sseItemBaseSchema);
    }
}
