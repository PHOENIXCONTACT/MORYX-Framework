// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Resources.Attributes;
using Moryx.Collections;
using Moryx.AspNetCore.Mqtt.Builders;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.Threading;
using Moryx.Tools;
using MQTTnet;
using MQTTnet.Packets;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Sends messages to the broker when the <see cref="Resource.Changed"/> is raised
/// or the <see cref="IResourceManagementChanges.ResourceChanged"/> is raised
/// </summary>
/// <param name="resourceManagementChanges"></param>
/// <param name="logger"></param>
/// <param name="parallelOperations"></param>
public class ResourceSynchronizationService : IMqttService
{
    private readonly int _cleanupIntervalMs = 60 * 1000;
    // TODO: these routes should be configurable in the future
    const string ResourceChangedTopic = "moryx/resources/changed";
    const string ResourceAddedTopic = "moryx/resources/added";
    const string ResourceRemovedTopic = "moryx/resources/removed";
    private WriteOnlyHashStore<string>? _messageCache;
    private readonly IManagedMqttClient _client;
    private readonly IResourceManagement _resourceManagement;
    private readonly ILogger<ResourceSynchronizationService> _logger;
    private readonly IParallelOperations _parallelOperations;
    private readonly MqttClientUserOptions _options;

    public ResourceSynchronizationService(
        IManagedMqttClient client,
        IResourceManagement resourceManagement,
        ILogger<ResourceSynchronizationService> logger,
        IParallelOperations parallelOperations,
        MqttClientUserOptions options)
    {
        ArgumentNullException.ThrowIfNull(resourceManagement);
        ArgumentNullException.ThrowIfNull(parallelOperations);
        _resourceManagement = resourceManagement;
        _parallelOperations = parallelOperations;
        _client = client;
        _logger = logger;
        _options = options;
    }

    #region IHostedService
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageCache = new WriteOnlyHashStore<string>(_parallelOperations);
        _messageCache.Initialize(_cleanupIntervalMs);

        _client.ConnectedAsync += OnConnectedAsync;
        _client.DisconnectedAsync += OnDisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.ConnectedAsync -= OnConnectedAsync;
        _client.DisconnectedAsync -= OnDisconnectedAsync;
        _client.ApplicationMessageReceivedAsync -= OnMessageReceivedAsync;
        _messageCache?.Dispose();
        return Task.CompletedTask;
    }

    #endregion

    #region Mqtt Client State Events
    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _resourceManagement.ResourceAdded -= ResourceAdded;
        _resourceManagement.ResourceRemoved -= ResourceRemoved;
        _resourceManagement.ResourceChanged -= ResourceChanged;
        return Task.CompletedTask;
    }

    private Task OnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _resourceManagement.ResourceAdded += ResourceAdded;
        _resourceManagement.ResourceRemoved += ResourceRemoved;
        _resourceManagement.ResourceChanged += ResourceChanged;

        _client.SubscribeAsync([
        new MqttTopicFilter
        {
            Topic = ResourceChangedTopic,
            NoLocal = _options.DefaultTopicStrategy.NoLocal,
            RetainAsPublished = _options.DefaultTopicStrategy.RetainAsPublished
        },
        new MqttTopicFilter
        {
            Topic = ResourceAddedTopic,
            NoLocal = _options.DefaultTopicStrategy.NoLocal,
            RetainAsPublished = _options.DefaultTopicStrategy.RetainAsPublished
        },
        new MqttTopicFilter
        {
            Topic = ResourceRemovedTopic,
            NoLocal = _options.DefaultTopicStrategy.NoLocal,
            RetainAsPublished = _options.DefaultTopicStrategy.RetainAsPublished
        }
        ], CancellationToken.None);
        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        => arg.ApplicationMessage.Topic switch
        {
            ResourceChangedTopic => HandleResourceChangedMessageAsync(arg),
            ResourceAddedTopic => HandleResourceAddedMessageAsync(arg),
            ResourceRemovedTopic => HandleResourceRemovedMessageAsync(arg),
            _ => Task.CompletedTask,
        };

    private async Task HandleResourceAddedMessageAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        if (!TryAddToMessageCache(GetHashString(arg.ApplicationMessage.ConvertPayloadToString())))
        {
            return;
        }

        var addedResourceObject = await MqttMessageSerialization
            .DeserializeAsync(arg.ApplicationMessage, _options.JsonSerializerOptions);
        if (addedResourceObject is not null && addedResourceObject is JsonElement document)
        {
            var jsonProperties = document.EnumerateObject();
            var synchronizationTypeId = jsonProperties.FirstOrDefault(x => x.Name == nameof(ResourceSynchronizationAttribute.SynchronizationTypeId));
            var synchronizationTypeIdValue = NullOrValue<string>(synchronizationTypeId.Value);
            var matchingResourceType = ReflectionTool.GetPublicClasses<Resource>(type => type.GetCustomAttribute<ResourceSynchronizationAttribute>()?.SynchronizationTypeId == synchronizationTypeIdValue).FirstOrDefault();
            if (matchingResourceType is null)
            {
                _logger.LogWarning("No resource type found for synchronization type id '{synchronizationTypeId}'. Resource creation skipped.", synchronizationTypeIdValue);
                return;
            }

            await _resourceManagement.CreateUnsafeAsync(matchingResourceType, async emptyResource =>
            {
                SetInstanceValue(jsonProperties, emptyResource);
                if (emptyResource is IIdentifiableObject identifiableObject)
                {
                    // Ensure that identity is set
                    var identityProperty = jsonProperties.FirstOrDefault(x => x.Name == nameof(IIdentifiableObject.Identity));
                    var identityValue = NullOrValue<BatchIdentity>(identityProperty.Value);
                    identifiableObject.Identity?.SetIdentifier(identityValue?.Identifier ?? string.Empty);
                }

                var message = MqttMessageSerialization.GetJsonPayload<IResource>(emptyResource, _options.JsonSerializerOptions);
                TryAddToMessageCache(GetHashString(message));
                return;
            });
        }
    }

    private async Task HandleResourceRemovedMessageAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        if (!TryAddToMessageCache(GetHashString(arg.ApplicationMessage.ConvertPayloadToString())))
        {
            return;
        }

        var removedResourceObject = await MqttMessageSerialization
            .DeserializeAsync(arg.ApplicationMessage, _options.JsonSerializerOptions);
        if (removedResourceObject is not null && removedResourceObject is JsonElement document)
        {
            await CompleteResourceProcessingAsync(document, ExistingResourceChangeType.Removed);
        }
    }

    private async Task HandleResourceChangedMessageAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        if (!TryAddToMessageCache(GetHashString(arg.ApplicationMessage.ConvertPayloadToString())))
        {
            return;
        }

        var updatedResource = await MqttMessageSerialization
            .DeserializeAsync(arg.ApplicationMessage, _options.JsonSerializerOptions);
        if (updatedResource is not null && updatedResource is JsonElement document)
        {
            await CompleteResourceProcessingAsync(document, ExistingResourceChangeType.Changed);
        }
    }

    private async Task CompleteResourceProcessingAsync(JsonElement document, ExistingResourceChangeType changeType)
    {
        var jsonProperties = document.EnumerateObject();
        var identity = jsonProperties.FirstOrDefault(X => X.Name == nameof(IIdentifiableObject.Identity));
        var name = jsonProperties.FirstOrDefault(X => X.Name == nameof(Resource.Name));

        //This is done just for casting purpose to access Identity.Identifier
        var identityValue = NullOrValue<BatchIdentity>(identity.Value);
        var nameValue = NullOrValue<string>(name.Value);
        var matchingResources = _resourceManagement
            .GetResources<IResource>(x =>
                x is IIdentifiableObject obj && obj.Identity.Identifier.Equals(identityValue?.Identifier)
                || x.Name.Equals(nameValue));
        if (matchingResources.Count() > 1)
        {
            _logger.LogWarning("Multiple resources found matching the identity '{identity}' or name '{name}'. Resource synchronization skipped.", identityValue?.Identifier, nameValue);
            return;
        }
        var matchingResource = matchingResources.FirstOrDefault();
        if (matchingResource is not null)
        {
            switch (changeType)
            {
                case ExistingResourceChangeType.Removed:
                    await RemoveLocalInstanceAsync(_resourceManagement, matchingResource);
                    return;
                case ExistingResourceChangeType.Changed:
                    await UpdateLocalInstanceAsync(_resourceManagement, jsonProperties, matchingResource);
                    return;
            }
        }
        else
        {
            _logger.LogWarning("No matching resource found for identity '{identity}' or name '{name}'. Resource synchronization skipped.", identityValue?.Identifier, nameValue);
        }

    }

    private bool TryAddToMessageCache(string messageHash)
    {
        // The message was sent by the current resource synchronization service, ignore it
        if (_messageCache?.Contains(messageHash) == true)
        {
            return false;
        }
        _messageCache?.Write(messageHash);
        return true;
    }

    private async Task UpdateLocalInstanceAsync(IResourceManagement resourceManagement, JsonElement.ObjectEnumerator jsonProperties, IResource matchingResource)
    {
        await resourceManagement.ModifyUnsafeAsync(matchingResource.Id, async resource =>
        {
            SetInstanceValue(jsonProperties, resource);
            return true;
        });
    }

    private async Task RemoveLocalInstanceAsync(IResourceManagement resourceManagement, IResource matchingResource)
    {
        var message = MqttMessageSerialization.GetJsonPayload<IResource>(matchingResource, _options.JsonSerializerOptions);
        TryAddToMessageCache(GetHashString(message));
        await resourceManagement.DeleteAsync(matchingResource.Id);
    }

    private void SetInstanceValue(JsonElement.ObjectEnumerator jsonProperties, Resource resource)
    {
        var resourceProperties = resource?.GetType().GetProperties() ?? [];
        foreach (var jsonProperty in jsonProperties)
        {
            var matchingProperty = resourceProperties
                .FirstOrDefault(x => x.Name == jsonProperty.Name);
            if (matchingProperty is not null)
            {
                try
                {
                    var propertyValue = NullOrValue(jsonProperty.Value, matchingProperty.PropertyType);
                    matchingProperty?.SetValue(resource, propertyValue);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while deserializing Mqtt Resource Synchronization Payload.", ex);
                }
            }
        }
    }

    private static T? NullOrValue<T>(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined
                    ? default
                    : element.Deserialize<T>();
    }

    private static object? NullOrValue(JsonElement element, Type valueType)
    {
        return element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined
                    ? default
                    : element.Deserialize(valueType);
    }

    #endregion

    #region Resource Events
    private void ResourceChanged(object? sender, IResource e)
    {
        _resourceManagement.ReadUnsafe(e.Id, resource =>
       {
           SendChanges(resource, ResourceChangedTopic);
           return resource;
       });
    }

    private void ResourceAdded(object? sender, IResource e)
    {
        _resourceManagement.ReadUnsafe(e.Id, resource =>
        {
            return ProcessResource(resource, ResourceAddedTopic);
        });

    }

    private void ResourceRemoved(object? sender, IResource e)
    {
        //e is a Proxy, so we have to use reflection to get the Identity
        var target = e.GetType().GetProperties()?.FirstOrDefault(x => x.Name == "Target")?.GetValue(e);
        var identity = target?.GetType().GetProperty("Identity")?.GetValue(target);
        var payload = new DeletedResourceMessage
        {
            SynchronizationTypeId = target?.GetType().GetCustomAttribute<ResourceSynchronizationAttribute>()?.SynchronizationTypeId,
            Identity = identity as IIdentity,
            Name = e.Name
        };
        var json = JsonSerializer.Serialize(payload, _options.JsonSerializerOptions);
        var hash = GetHashString(json);
        PublishMessage(ResourceRemovedTopic, json, hash);
        TryAddToMessageCache(hash);
    }

    /// <summary>
    /// Message for deleted resource synchronization
    /// </summary>
    private class DeletedResourceMessage
    {
        public string? SynchronizationTypeId { get; set; }
        public IIdentity? Identity { get; set; }
        public string? Name { get; set; }
    }

    private void SendChanges(Resource resource, string topic)
    {
        if (resource.GetType().GetCustomAttribute<ResourceSynchronizationAttribute>() is null)
        {
            return;
        }

        var messageString = MqttMessageSerialization.GetJsonPayload<IResource>(resource, _options.JsonSerializerOptions);
        var messageHash = GetHashString(messageString);
        PublishMessage(topic, messageString, messageHash);
    }

    private void PublishMessage(string topic, string messageString, string messageHash)
    {
        var messageBuilder = new MqttApplicationMessageBuilder();
        messageBuilder.WithTopic(topic);
        messageBuilder.WithPayload(messageString);

        if (_messageCache?.Contains(messageHash) == false)
        {
            _messageCache.Write(messageHash);
            // Publish the message using the Hosted MQTT
            _client.EnqueueAsync(messageBuilder.Build(), CancellationToken.None);
        }
    }

    private Resource ProcessResource(Resource resource, string topic)
    {
        var synchronizationAttribute = resource.GetType().GetCustomAttribute<ResourceSynchronizationAttribute>();
        if (synchronizationAttribute?.SynchronizationTypeId is null)
        {
            _logger.LogError("Resource of type {resourceType} is marked for synchronization but has no SynchronizationTypeId defined.", resource.GetType().FullName);
            return resource;
        }
        SendChanges(resource, topic);
        return resource;
    }

    private static string GetHashString(string inputString)
    {
        StringBuilder stringBuilder = new();
        foreach (var @byte in SHA256.HashData(Encoding.UTF8.GetBytes(inputString)))
        {
            stringBuilder.Append(@byte.ToString("X2"));
        }
        return stringBuilder.ToString();
    }

    enum ExistingResourceChangeType
    {
        Removed,
        Changed
    }
    #endregion
}
