// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    /// <summary>
    /// Converts ResourceModel to Resource
    /// </summary>
    internal class ModelToResourceConverter
    {
        /// <summary>
        /// Resource cache to avoid redundant conversions AND make use of WCFs "IsReference" feature
        /// </summary>
        private readonly Dictionary<long, Resource> _resourceCache = new Dictionary<long, Resource>();

        private readonly IResourceGraph _resourceGraph;
        private readonly ICustomSerialization _serialization;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceGraph"></param>
        /// <param name="serialization"></param>
        public ModelToResourceConverter(IResourceGraph resourceGraph, ICustomSerialization serialization)
        {
            _resourceGraph = resourceGraph;
            _serialization = serialization;
        }

        /// <summary>
        /// Convert ResourceModel back to resource and/or update its properties
        /// </summary>
        public Resource FromModel(ResourceModel model, HashSet<Resource> resourcesToSave, Resource resource = null)
        {
            // Break recursion if we converted this instance already
            // Try to load by real id first
            if (_resourceCache.ContainsKey(model.Id))
                return _resourceCache[model.Id];
            // Otherwise by reference id
            if (model.Id == 0 && _resourceCache.ContainsKey(model.ReferenceId))
                return _resourceCache[model.ReferenceId];

            // Only fetch resource object if it was not given
            if (resource == null)
            {
                resource = model.Id == 0
                    ? _resourceGraph.Instantiate(model.Type)
                    : _resourceGraph.Get(model.Id);
            }

            // Write to cache because following calls might only have an empty reference
            if (model.Id == 0)
                _resourceCache[model.ReferenceId] = resource;
            else
                _resourceCache[model.Id] = resource;

            // Do not copy values from partially loaded models
            if (model.PartiallyLoaded)
                return resource;

            // Add to list if object was created or modified
            if (model.Id == 0 || model.DifferentFrom(resource, _serialization))
                resourcesToSave.Add(resource);

            // Copy standard properties
            resource.Name = model.Name;
            resource.Description = model.Description;

            // Copy extended properties
            EntryConvert.UpdateInstance(resource.Descriptor, model.Properties, _serialization);

            // Set all other references
            UpdateReferences(resource, resourcesToSave, model);

            return resource;
        }

        /// <summary>
        /// Updates the references of a resource
        /// </summary>
        private void UpdateReferences(Resource instance, HashSet<Resource> resourcesToSave, ResourceModel model)
        {
            var type = instance.GetType();
            foreach (var reference in model.References)
            {
                // Skip references that were not set
                if (reference.Targets == null)
                    continue;

                var property = type.GetProperty(reference.Name);
                if (property.GetValue(instance) is IReferenceCollection asCollection)
                {
                    var collection = asCollection.UnderlyingCollection;
                    // Add new items and update existing ones
                    foreach (var targetModel in reference.Targets)
                    {
                        Resource target;
                        if (targetModel.Id == 0 || collection.All(r => r.Id != targetModel.Id))
                        {
                            // New reference added to the collection
                            target = FromModel(targetModel, resourcesToSave);
                            collection.Add(target);
                            resourcesToSave.Add(instance);
                        }
                        else
                        {
                            // Element already exists in the collection
                            target = (Resource)collection.First(r => r.Id == targetModel.Id);
                            FromModel(targetModel, resourcesToSave, target);
                        }
                    }
                    // Remove deleted items
                    var targetIds = reference.Targets.Select(t => t.Id).Distinct().ToArray();
                    var deletedItems = collection.Where(r => !targetIds.Contains(r.Id)).ToArray();
                    foreach (var deletedItem in deletedItems)
                        collection.Remove(deletedItem);

                    if (deletedItems.Any())
                    {
                        resourcesToSave.Add(instance);
                    }
                }
                else
                {
                    var targetModel = reference.Targets.FirstOrDefault();
                    var value = (Resource)property.GetValue(instance);

                    Resource target;
                    if (targetModel == null)
                        target = null;
                    else if (targetModel.Id == value?.Id)
                        target = FromModel(targetModel, resourcesToSave, value);
                    else
                        target = FromModel(targetModel, resourcesToSave);

                    if (target != value)
                    {
                        property.SetValue(instance, target);
                        resourcesToSave.Add(instance);
                    }
                }
            }
        }
    }
}
