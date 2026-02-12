// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources.Attributes;
using Moryx.Tools;

namespace Moryx.AbstractionLayer.Resources.Converters;

/// <summary>
/// Converts any <see cref="IResource"/> that has the <see cref="ResourceSynchronizationAttribute"/> to JSON
/// </summary>
/// <param name="resourceManagement">Resource Management facade</param>
public class ResourceToJsonConverter(IResourceManagement resourceManagement)
    : JsonConverter<IResource>
{
    //</inherit>
    public override IResource? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        string? identifier = null;
        var jsonObj = JsonNode.Parse(ref reader)!.AsObject();
        var name = jsonObj[nameof(Resource.Name)]?.ToString();
        var identityObj = jsonObj[nameof(Identity)]?.AsObject();
        if (identityObj is not null)
        {
            identifier = identityObj[nameof(IIdentity.Identifier)].ToString();
        }
        var matchingResource = resourceManagement
            .GetResource<Resource>(x =>
                    x is IIdentifiableObject identifiable && identifiable.Identity.Identifier.Equals(identifier)
                    || x.Name.Equals(name));

        return matchingResource is null
            ? HandleUnknownResource(jsonObj, options)
            : jsonObj.Deserialize(matchingResource.GetType(), options) as Resource;
    }

    private Resource HandleUnknownResource(JsonObject jsonObj, JsonSerializerOptions options)
    {
        var synchronizationTypeId = jsonObj[nameof(ResourceSynchronizationAttribute.SynchronizationTypeId)]?.ToString();
        if (synchronizationTypeId is null)
        {
            throw new InvalidOperationException("Cannot handle unknown resource without SynchronizationTypeId");
        }

        var matchingResourceType = resourceManagement.GetResourcesUnsafe<Resource>(x => x.GetType().GetCustomAttribute<ResourceSynchronizationAttribute>()?.SynchronizationTypeId == synchronizationTypeId)
            .FirstOrDefault()?
            .GetType();
        return matchingResourceType is null
            ? throw new InvalidOperationException($"No resource type found for SynchronizationTypeId {synchronizationTypeId}")
            : jsonObj.Deserialize(matchingResourceType, options) as Resource;
    }

    //</inherit>
    public override void Write(Utf8JsonWriter writer, IResource value, JsonSerializerOptions options)
    {
        var synchronizationAttribute = value.GetType().GetCustomAttribute<ResourceSynchronizationAttribute>();

        if (synchronizationAttribute is null)
        {
            return;
        }

        var actualType = value.GetType();
        if (synchronizationAttribute.Mode == SynchronizationMode.Full)
        {
            var json = JsonSerializer.SerializeToElement(value, actualType, options);

            writer.WriteStartObject();
            var jsonProperties = GetJsonProperties(json, actualType);

            foreach (var property in jsonProperties)
            {
                property.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
        else
        {
            var selectedProperties = actualType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SynchronizableMemberAttribute>() is not null
                || x.Name == nameof(Resource.Name)
                || x.Name == nameof(Resource.Description)) ?? [];
            var jsonObject = new JsonObject
            {
                { nameof(ResourceSynchronizationAttribute.SynchronizationTypeId), ConvertToJsonNode(synchronizationAttribute.SynchronizationTypeId, typeof(string), options) }
            };
            foreach (var property in selectedProperties)
            {
                var valueObj = property.GetValue(value, null);
                jsonObject.Add(property.Name, ConvertToJsonNode(valueObj, property.PropertyType, options));
            }
            jsonObject.WriteTo(writer);
        }
    }

    private static IEnumerable<JsonProperty> GetJsonProperties(JsonElement json, Type actualType)
    {
        //General cleanup
        var properties = json.EnumerateObject().Where(x =>
              x.Name != nameof(Resource.Descriptor)
              && x.Name != nameof(Resource.Id)
              && x.Name != nameof(Resource.Capabilities)
              && x.Name != nameof(Resource.Logger)
              && x.Name != nameof(Resource.Graph)
              && x.Name != "$id");

        //TODO: in the future we can shallow serialization only consisting of the identity(List<Identity>) ?
        var referenceProperties = actualType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .Where(x => x.GetCustomAttribute<ResourceReferenceAttribute>() is not null
            || x.PropertyType == typeof(IReferences<>));
        return properties.Where(x => !referenceProperties.Any(r => r.Name == x.Name));
    }

    private static JsonNode? ConvertToJsonNode(object? value, Type type, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return null;
        }
        else
        {
            if (type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(Guid))
            {
                return JsonValue.Create(value);
            }
            else
            {
                return JsonSerializer.SerializeToNode(value, options);
            }
        }
    }

}