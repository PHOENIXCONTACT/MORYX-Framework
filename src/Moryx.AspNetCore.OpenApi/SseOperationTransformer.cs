// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Moryx.AspNetCore;

internal class SseOperationTransformer(IOptions<OpenApiOptions> openapiOptions) : IOpenApiOperationTransformer
{
    internal const string SseItemBaseSchemaId = "SseItemBase";
    private const string EventStreamResponseType = "text/event-stream";
    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var producesAttributes = context.Description.ActionDescriptor.EndpointMetadata.OfType<ProducesSseEventAttribute>().ToList();

        if (producesAttributes.Count == 0)
        {
            return;
        }

        foreach (var attribute in producesAttributes)
        {

        }
        var pseudoResponseKeys = operation.Responses.Keys.Where(x => int.TryParse(x, out var v) && v >= 1000).ToList();
        foreach (var pseudoKeys in pseudoResponseKeys)
        {
            operation.Responses.Remove(pseudoKeys);
        }

        var responseKey = ((int)HttpStatusCode.OK).ToString(CultureInfo.InvariantCulture);
        if (!operation.Responses.TryGetValue(responseKey, out var response))
        {
            response = new OpenApiResponse()
            {
                Description = HttpStatusCode.OK.GetDisplayName(),
                Content = new Dictionary<string, OpenApiMediaType>(),
            };

            operation.Responses.Add(responseKey, response);
        }
        if (!response.Content.TryGetValue(EventStreamResponseType, out var mediaType))
        {
            mediaType = new OpenApiMediaType();
            response.Content[EventStreamResponseType] = mediaType;
        }

        Dictionary<string, OpenApiSchemaReference> schemas = new();
        foreach (var a in producesAttributes)
        {
            var dataSchemaId = openapiOptions.Value.CreateSchemaReferenceId(JsonTypeInfo.CreateJsonTypeInfo(a.PayloadType, new JsonOptions().SerializerOptions));
            Debug.Assert(context.Document.Components.Schemas.ContainsKey(dataSchemaId));

            var schemaId = $"SseItemOf{a.PayloadType.Name}";
            if (!context.Document.Components.Schemas.TryGetValue(schemaId, out var schema))
            {
                context.Document.AddComponent(schemaId, new OpenApiSchema()
                {
                    AllOf = [new OpenApiSchemaReference(SseItemBaseSchemaId)],
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        { "data", new OpenApiSchemaReference(dataSchemaId) }
                    }
                });
            }
            var description = string.IsNullOrEmpty(a.Description)
                ? $"Published with type '{a.EventType}'"
                : $"Published with type '{a.EventType}': {a.Description}";

            schemas.Add(a.EventType, new OpenApiSchemaReference(schemaId) { Description = description });
        }

        mediaType.Schema = new OpenApiSchema()
        {
            Discriminator = new OpenApiDiscriminator()
            {
                PropertyName = "type",
                Mapping = schemas,
            },
            OneOf = schemas.Values.ToList<IOpenApiSchema>(),
        };

    }
}
