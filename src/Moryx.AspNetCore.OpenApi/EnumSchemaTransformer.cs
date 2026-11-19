// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

internal sealed class EnumSchemaTransformer : IOpenApiSchemaTransformer
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

        // try to set schema.Type because ng-openapi-gen will create unions instead of enums if not set
        if (schema.Type == null)
        {
            if (schema.Enum.All(e => e.GetValueKind() == JsonValueKind.String))
            {
                schema.Type = JsonSchemaType.String;
            }
            else if (schema.Enum.All(e => e.GetValueKind() == JsonValueKind.Number))
            {
                schema.Type = JsonSchemaType.Number;
            }
        }
    }
    public async Task SetEnumValuesForFlagEnums(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Enum != null || context.JsonTypeInfo.Type.GetCustomAttributes<FlagsAttribute>() is null)
        {
            return;
        }
        var mvcJsonOptions = context.ApplicationServices.GetService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>();
        var minimalApiJsonOptions = context.ApplicationServices.GetService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>();

        var mvcUsesStringNames = mvcJsonOptions?.Value?.JsonSerializerOptions.Converters.Any(c => c is JsonStringEnumConverter converter);
        var minimalApiUsesStringNames = minimalApiJsonOptions?.Value?.SerializerOptions.Converters.Any(c => c is JsonStringEnumConverter converter);

        List<JsonNode> cases = [];
        if (mvcUsesStringNames == true || minimalApiUsesStringNames == true)
        {
            cases.AddRange(Enum.GetNames(context.JsonTypeInfo.Type).Select(t => JsonValue.Create(t)).ToArray());
        }
        if (mvcUsesStringNames == false || minimalApiUsesStringNames == false)
        {
            cases.AddRange(Enum.GetValuesAsUnderlyingType(context.JsonTypeInfo.Type).OfType<int>().Select(t => JsonValue.Create(t)));
        }
        schema.Enum = cases;
    }
}
