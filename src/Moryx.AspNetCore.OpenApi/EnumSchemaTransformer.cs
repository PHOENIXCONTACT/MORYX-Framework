// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

internal class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (!context.JsonTypeInfo.Type.IsEnum)
        {
            return;
        }
        // Set enum values for flage nums, because otherwise the information about possible keys is completely lost in the spec
        // It's not really correct, because technically other values than the pure enum cases are accepted
        await SetEnumValuesForFlagEnums(schema, context, cancellationToken);

        // set schema.Type because ng-openapi-gen will create unions instead of enums if not set
        if (schema.Type == null && schema.Enum.All(e => e.GetValueKind() == JsonValueKind.String))
        {
            schema.Type = JsonSchemaType.String;
        }
    }
    public async Task SetEnumValuesForFlagEnums(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Enum != null || context.JsonTypeInfo.Type.GetCustomAttributes<FlagsAttribute>() is null)
        {
            return;
        }
        schema.Enum = Enum.GetNames(context.JsonTypeInfo.Type)
            .Select(t => JsonValue.Create(t)).ToArray();
    }
}
