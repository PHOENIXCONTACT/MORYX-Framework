// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

/// <summary>
/// Controllers by default advertise application/json, "text/json" and "application/*+json"
/// as accepted body content and if not specified using a Produces Attribute the responses
/// can contain application/json, text/json and text/plain. This transformer deduplicates at least
/// all the different forms of json.
/// </summary>
internal class DeduplicateJsonContentTransformer : IOpenApiOperationTransformer
{
    private static readonly string[] unwantedJsonAlternatives = [
        "application/*+json", "text/json"
    ];

    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        RemoveDuplicateContent(operation.RequestBody?.Content);
        foreach (var content in operation.Responses.Values.Select(r => r.Content))
        {
            RemoveDuplicateContent(content);
        }
    }

    private void RemoveDuplicateContent(IDictionary<string, OpenApiMediaType> content)
    {
        if (content == null)
        {
            return;
        }
        if (!content.TryGetValue("application/json", out var appJsonMedia))
        {
            return;
        }

        foreach (var alternativeName in unwantedJsonAlternatives)
        {
            if (!content.TryGetValue(alternativeName, out var extraJsonMedia))
            {
                continue;
            }
            // Try to check that the schemas are actually the same before removing alternative schemas
            if (SchemasMatch(appJsonMedia.Schema, extraJsonMedia?.Schema))
            {
                content.Remove(alternativeName);
            }
        }
    }

    private bool SchemasMatch(IOpenApiSchema appJsonSchema, IOpenApiSchema alternative)
    {
        if (appJsonSchema == null)
        {
            return alternative == null;
        }
        switch (appJsonSchema.Type)
        {
            // Handle simple types
            case JsonSchemaType.String or JsonSchemaType.Number or JsonSchemaType.Null or JsonSchemaType.Boolean:
                return alternative?.Type == appJsonSchema.Type;
            case JsonSchemaType.Object:
                if (appJsonSchema is not OpenApiSchemaReference schemaRef || schemaRef.Reference?.ReferenceV3 == null)
                {
                    return false;
                }
                return alternative is OpenApiSchemaReference alternativeRef && alternativeRef.Reference?.ReferenceV3 == schemaRef.Reference?.ReferenceV3;
            case JsonSchemaType.Array:
                return alternative?.Type == JsonSchemaType.Array && SchemasMatch(appJsonSchema.Items, alternative.Items);
            default:
                // This should normally not be hit
                return false;
        }
    }

}
