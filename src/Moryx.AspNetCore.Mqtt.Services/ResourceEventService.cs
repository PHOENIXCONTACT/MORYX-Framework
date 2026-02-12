// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.AspNetCore.Mqtt.Builders;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.Runtime.Modules;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Sends messages to the broker when a public event or event exposed using the <see cref="ResourceAvailableAsAttribute"/>
/// is raised
/// </summary>
/// <param name="resourceManagement">Resource management facade</param>
public class ResourceEventService : IMqttService
{
    class ResourceEventSubscription
    {
        public object Resource { get; }
        public string EventName { get; }
        public Delegate Handler { get; }

        public ResourceEventSubscription(object resource, string eventName, Delegate handler)
        {
            Resource = resource;
            EventName = eventName;
            Handler = handler;
        }
    }

    private readonly IManagedMqttClient _client;
    private readonly IResourceManagement _resourceManagement;
    private readonly MqttClientUserOptions _options;
    private readonly Dictionary<long, List<ResourceEventSubscription>> _eventHandlers = new();
    public ResourceEventService(
        IManagedMqttClient client,
        IResourceManagement resourceManagement,
        MqttClientUserOptions options)
    {
        ArgumentNullException.ThrowIfNull(resourceManagement);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);
        _resourceManagement = resourceManagement;
        _client = client;
        _options = options;
    }

    #region Hosted Service
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_resourceManagement is ILifeCycleBoundFacade lifeCycleBoundFacade)
        {
            lifeCycleBoundFacade.StateChanged += FacadeStateChanged;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        UnsubscribeFromResourcesEvents();
        if (_resourceManagement is ILifeCycleBoundFacade lifeCycleBoundFacade)
        {
            lifeCycleBoundFacade.StateChanged -= FacadeStateChanged;
        }
        return Task.CompletedTask;
    }
    #endregion

    private void FacadeStateChanged(object? sender, bool activated)
    {
        if (activated)
        {
            SubscribeToResourcesEvents();
        }
        else
        {
            UnsubscribeFromResourcesEvents();
        }
    }

    private void UnsubscribeFromResourcesEvents()
    {
        var resources = _resourceManagement.GetResources<IResource>(resource => resource is not null) ?? [];
        foreach (var resource in resources)
        {
            RemoveEventHandlers(resource);
        }
        _resourceManagement.ResourceAdded -= ResourceManagement_ResourceAdded;
        _resourceManagement.ResourceRemoved -= ResourceManagement_ResourceRemoved;
    }

    private void SubscribeToResourcesEvents()
    {
        var resources = _resourceManagement.GetResources<IResource>(resource => resource is not null) ?? [];
        foreach (var resource in resources)
        {
            CreateEventHandlers(resource);
        }
        _resourceManagement.ResourceAdded += ResourceManagement_ResourceAdded;
        _resourceManagement.ResourceRemoved += ResourceManagement_ResourceRemoved;
    }

    private void ResourceManagement_ResourceRemoved(object? sender, IResource e)
    {
        RemoveEventHandlers(e);
    }

    private void ResourceManagement_ResourceAdded(object? sender, IResource e)
    {
        CreateEventHandlers(e);
    }

    private void CreateEventHandlers(IResource resource)
    {
        var eventHandlerAddMethods = resource.GetType()
            .GetInterfaces()
            .SelectMany(x => x.GetMethods().Where(w => w.Name.StartsWith("add_")));
        if (!eventHandlerAddMethods.Any())
        {
            return;
        }

        foreach (var eventAddHandlerMethod in eventHandlerAddMethods)
        {
            var eventHelper = typeof(ResourceEventHandler<>);
            var typeOfTheGeneric = eventAddHandlerMethod
                .GetParameters()[0]
                .ParameterType
                .GetMethod("Invoke")?
                .GetParameters()[1]
                .ParameterType;

            if (typeOfTheGeneric == null)
            {
                continue;
            }
            var helperInstance = eventHelper.MakeGenericType(typeOfTheGeneric);

            var eventHandler = helperInstance
                .GetMethod(nameof(ResourceEventHandler<object>.InvokeMethod));
            if (eventHandler == null)
            {
                continue;
            }

            var eventName = eventAddHandlerMethod.Name.Replace("add_", "");
            var identifier = resource is IIdentifiableObject obj ? obj.Identity.Identifier : resource.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var responseTopic = string.IsNullOrEmpty(_options.Connection.RootTopic)
                                ? $"resources/{identifier}/event/{eventName}"
                                : $"{_options.Connection.RootTopic}/resources/{identifier}/event/{eventName}";
            var target = Activator.CreateInstance(helperInstance,
                    _client,
                    _options.JsonSerializerOptions,
                    responseTopic,
                    eventName
                );

            var newDelegate = Delegate.CreateDelegate(
                type: eventAddHandlerMethod.GetParameters()[0].ParameterType,
                firstArgument: target,
                method: eventHandler);
            eventAddHandlerMethod.Invoke(resource, [newDelegate]);

            // Store the delegate for later removal
            if (!_eventHandlers.TryGetValue(resource.Id, out var existingHandlers))
            {
                existingHandlers = [];
                _eventHandlers[resource.Id] = existingHandlers;
            }
            existingHandlers.Add(new ResourceEventSubscription(resource, eventName, newDelegate));
        }
    }

    /// <summary>
    /// Removes all event handlers that were created for the specified resource
    /// </summary>
    /// <param name="resource">The resource to remove event handlers from</param>
    private void RemoveEventHandlers(IResource resource)
    {
        if (!_eventHandlers.TryGetValue(resource.Id, out var eventSubscriptions))
        {
            return;
        }

        foreach (var subscription in eventSubscriptions)
        {
            // Find the corresponding remove method for the event
            var eventRemoveHandlerMethod = resource.GetType()
                .GetInterfaces()
                .SelectMany(x => x.GetMethods())
                .FirstOrDefault(m => m.Name == $"remove_{subscription.EventName}");

            eventRemoveHandlerMethod?.Invoke(resource, [subscription.Handler]);
        }

        // Remove the stored handlers for this resource
        _eventHandlers.Remove(resource.Id);
    }

}
