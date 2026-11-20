// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

/// <summary>
/// If a controller is annotated with application/json and the Endpoint method returns
/// an ActionResult and no specific response annotation is present the schema contains
/// an entry for the success status code and application/json, without any schema definition.
/// 
/// Swagger used to prune those from our api spec and therefore the UIs expect these endpoints to return void.
/// If we don't prune it too the UI has compiler errors, because "any" is now returned instead of void.
/// </summary>
internal sealed class RemoveEmptyResponseTypeOperationTransformer : IOpenApiOperationTransformer
{
    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        foreach (var (code, value) in operation.Responses)
        {
            var emptyTypes =
                value?.Content?
                    .Where(responseEntry => responseEntry.Value.Schema == null).ToArray()
                    ?? [];

            foreach (var empty in emptyTypes)
            {
                value.Content.Remove(empty.Key);
            }
        }
    }
}
