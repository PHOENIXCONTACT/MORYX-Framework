// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

/// <summary>
/// Remove content types that don't have a defined response in the schema.
/// Necessary to keep the void results in generated client code.
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
