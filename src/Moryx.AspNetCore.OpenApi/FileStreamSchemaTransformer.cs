// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO.Pipelines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

internal class FileStreamSchemaTransformer : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;
        // Lightly adapted from https://github.com/dotnet/aspnetcore/blob/2e65b75b42210ac4e9f8314e0d6910ede3035dad/src/OpenApi/src/Services/Schemas/OpenApiSchemaService.cs#L62
        // Fix up schemas generated for IFormFile, IFormFileCollection, Stream, PipeReader,
        // FileContentResult, FileStreamResult, FileContentHttpResult and FileStreamHttpResult
        // that appear as properties within complex types.
        if (type == typeof(IFormFile) || type == typeof(Stream) || type == typeof(PipeReader)
            || type == typeof(FileContentResult) || type == typeof(FileStreamResult)
            || type == typeof(FileContentHttpResult) || type == typeof(FileStreamHttpResult))
        {
            schema.Type = JsonSchemaType.String;
            schema.Properties = null;
            schema.Format = "binary";
        }
        else if (type == typeof(IFormFileCollection))
        {
            schema.Type = JsonSchemaType.Array;
            schema.Items = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "binary",
            };
        }
    }
}
