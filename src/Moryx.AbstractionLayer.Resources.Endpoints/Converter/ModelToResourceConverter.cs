// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources.Endpoints.Models;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Converts ResourceModel to Resource
/// </summary>
internal sealed class ModelToResourceConverter
{
    private readonly ICustomSerialization _serialization;

    private readonly IResourceTypeTree _resourceTypeTree;

    private readonly IResourceManagement _resourceManagement;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resourceGraph"></param>
    /// <param name="serialization"></param>
    public ModelToResourceConverter(IResourceManagement resourceManagement, IResourceTypeTree resourceTypeTree, ICustomSerialization serialization)
    {
        _resourceManagement = resourceManagement;
        _resourceTypeTree = resourceTypeTree;
        _serialization = serialization;
    }
    /// <summary>
    /// Convert ResourceModel back to resource and/or update its properties
    /// </summary>
    public async Task<Resource> FromModel(ResourceModel model, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, Resource resource = null)
    {
        // Break recursion if we converted this instance already
        // Try to load by real id first
        if (cache.TryGetValue(model.Id, out var value))
        {
            return value;
        }
        // Otherwise by reference id
        if (model.Id == 0 && cache.TryGetValue(model.ReferenceId, out var referencedValue))
        {
            return referencedValue;
        }

        // Only fetch resource object if it was not given
        if (resource is null)
        {
            if (model.Id == 0 && model.PartiallyLoaded)
            {
                resource = (Resource)Activator.CreateInstance(_resourceTypeTree[model.Type].ResourceType);
            }
            else if (model.Id == 0)
            {
                var id = await _resourceManagement.CreateUnsafeAsync(_resourceTypeTree[model.Type].ResourceType, r => Task.CompletedTask);
                resource = _resourceManagement.ReadUnsafe(id, r => r);
            }
            else
            {
                resource = _resourceManagement.ReadUnsafe(model.Id, r => r);
            }
        }

        // Write to cache because following calls might only have an empty reference
        if (model.Id == 0)
        {
            cache[model.ReferenceId] = resource;
        }
        else
        {
            cache[model.Id] = resource;
        }

        // Do not copy values from partially loaded models
        if (model.PartiallyLoaded)
        {
            return resource;
        }

        // Add to list if object was created or modified
        if (model.Id == 0 || model.DifferentFrom(resource, _serialization))
        {
            resourcesToSave.Add(resource.Id);
        }

        // Copy standard properties
        resource.Name = model.Name;
        resource.Description = model.Description;

        // Copy extended properties
        EntryConvert.UpdateInstance(resource.Descriptor, model.Properties, _serialization);

        // Set all other references
        await UpdateReferences(resource, resourcesToSave, cache, model);

        return resource;
    }

    /// <summary>
    /// Updates the references of a resource
    /// </summary>
    public async Task UpdateReferences(Resource instance, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, ResourceModel model)
    {
        var type = instance.GetType();
        foreach (var reference in model.References)
        {
            // Skip references that were not set
            if (reference.Targets == null)
            {
                continue;
            }

            var property = type.GetProperty(reference.Name);
            if (property.GetValue(instance) is IReferenceCollection asCollection)
            {
                bool collectionChanged = false;
                var collection = asCollection.UnderlyingCollection;

                // TODO: find a way to synchronize collection acceess for adding as well
                // currently we can't do it, because of the await statements.

                // Add new items and update existing ones
                foreach (var targetModel in reference.Targets)
                {
                    Resource target;
                    if (targetModel.Id == 0 || collection.All(r => r.Id != targetModel.Id))
                    {
                        // New reference added to the collection
                        target = await FromModel(targetModel, resourcesToSave, cache);
                        collection.Add(target);
                        collectionChanged = true;
                        resourcesToSave.Add(target.Id);
                    }
                    else
                    {
                        // Element already exists in the collection
                        target = (Resource)collection.First(r => r.Id == targetModel.Id);
                        await FromModel(targetModel, resourcesToSave, cache, target);
                    }
                }
                // Remove deleted items
                var targetIds = reference.Targets.Select(t => t.Id).Distinct().ToArray();
                lock (collection)
                {
                    var deletedItems = collection.Where(r => !targetIds.Contains(r.Id)).ToArray();
                    foreach (var deletedItem in deletedItems)
                    {
                        collectionChanged = true;
                        collection.Remove(deletedItem);
                    }

                    if (deletedItems.Any())
                    {
                        resourcesToSave.Add(instance.Id);
                    }
                }
                if (collectionChanged && collection is IReferenceCollection extended)
                {
                    extended.UnderlyingCollectionChanged();
                }
            }
            else
            {
                var targetModel = reference.Targets.FirstOrDefault();
                var value = (Resource)property.GetValue(instance);

                Resource target;
                if (targetModel == null)
                {
                    target = null;
                }
                else if (targetModel.Id == value?.Id)
                {
                    target = await FromModel(targetModel, resourcesToSave, cache, value);
                }
                else
                {
                    target = await FromModel(targetModel, resourcesToSave, cache);
                }

                if (target != value)
                {
                    property.SetValue(instance, target);
                    resourcesToSave.Add(instance.Id);
                }
            }
        }
    }
}
