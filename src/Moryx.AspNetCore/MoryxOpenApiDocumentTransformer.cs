// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore;

public class MoryxOpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string SseItemBaseSchemaId = "SseItemBase";
    private const string EventStreamResponseType = "text/event-stream";

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        SortPathEntries(document);

        EnsureSseItemBaseExists(document);

        foreach (var (path, pathEntry) in document.Paths)
        {
            var pathString = new string(path.AsSpan(1));
            foreach (var (method, operation) in (pathEntry.Operations))
            {
                var description = context.DescriptionGroups.SelectMany(dg => dg.Items)
                    .FirstOrDefault(description =>
                        description.RelativePath == pathString && description.HttpMethod == method.Method); // Find a better way to populate

                await UpdateOperation(operation, new OpenApiOperationTransformerContext()
                {
                    ApplicationServices = context.ApplicationServices,
                    Document = document,
                    DocumentName = context.DocumentName,
                    Description = description,
                }, cancellationToken);
            }
        }
    }

    private static void SortPathEntries(OpenApiDocument document)
    {
        var path = new OpenApiPaths();
        var sortLookup = new Dictionary<string, int>();
        document.Tags = document.Tags.OrderBy(t => t.Name).ToHashSet();

        var sorted = document.Paths.OrderBy((p) =>
        {
            if (p.Value.Operations.Count == 0)
            {
                return int.MaxValue;
            }

            var op = p.Value.Operations.First().Value;
            var tag = op.Tags.FirstOrDefault()?.Name ?? "";
            tag = tag.ToLower();
            if (sortLookup.TryGetValue(tag, out var key))
            {
                return key;
            }

            Span<byte> b = stackalloc byte[8];
            Encoding.UTF8.GetBytes(tag.AsSpan(0, Math.Min(4, tag.Length)), b);
            (b[0], b[1], b[2], b[3]) = (b[3], b[2], b[1], b[0]);
            key = BitConverter.ToInt32(b);
            sortLookup[tag] = key;
            return key;
        });

        foreach (var item in sorted)
        {
            path.Add(item.Key, item.Value);
        }

        document.Paths = path;
    }

    public async Task UpdateOperation(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        SetOperationIds(operation, context);

        await AddSseDocs(operation, context);
    }

    private static async Task AddSseDocs(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        var producesAttributes = context.Description.ActionDescriptor.EndpointMetadata.OfType<ProducesSseEventAttribute>().ToList();

        if (producesAttributes.Count == 0)
        {
            return;
        }

        var responseKey = ((int)HttpStatusCode.OK).ToString(CultureInfo.InvariantCulture);
        if (!operation.Responses.TryGetValue(responseKey, out var response))
        {
            response = new OpenApiResponse();
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
            var dataSchema = await context.GetOrCreateSchemaAsync(a.PayloadType);

            var dataSchemaId = dataSchema.Metadata.TryGetValue("x-schema-id", out var x) && x is string id ? id : a.PayloadType.Name;
            if (!context.Document.Components.Schemas.ContainsKey(dataSchemaId))
            {
                context.Document.AddComponent(dataSchemaId, dataSchema);
            }

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

    private static void SetOperationIds(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        if (context.Description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            operation.OperationId = controllerActionDescriptor.MethodInfo.Name; // TODO: This keeps the old behavior, but it's actually violating guidelines regrading uniqueness
        }
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
        document.AddComponent(SseItemBaseSchemaId, sseItemBaseSchema);
    }
}
