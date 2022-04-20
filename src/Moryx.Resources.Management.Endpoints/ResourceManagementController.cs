using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.Interaction;
using Moryx.Resources.Interaction.Converter;
using Moryx.Serialization;
using Moryx.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Moryx.Resources.Management.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IResourceManagement"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/resources/")]
    public class ResourceModificationController : ControllerBase
    {
        private readonly IResourceModification _resourceModification;
        private readonly IResourceTypeTree _resourceTypeTree;
        private readonly ResourceSerialization _serialization;

        public ResourceModificationController(IResourceModification resourceModification)
        {
            _resourceModification = resourceModification ?? throw new ArgumentNullException(nameof(resourceModification));
            _resourceTypeTree = _resourceModification as IResourceTypeTree ?? throw new InvalidCastException(nameof(resourceModification));
            _serialization = new ResourceSerialization();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("types")]
        public ActionResult<ResourceTypeModel> GetTypeTree()
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
            return converter.ConvertType(_resourceTypeTree.RootType);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        public ActionResult<ResourceModel[]> GetDetailsBatch([FromQuery] long[] ids)
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);

            if (ids is null)
                ids = _resourceModification.GetResources<IPublicResource>().Select(r => r.Id).ToArray();

            return ids.Select(id => _resourceModification.Read(id, r => converter.GetDetails(r)))
                .Where(details => details != null).ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("query")]
        public ActionResult<ResourceModel[]> GetResources(ResourceQuery query)
        {
            var filter = new ResourceQueryFilter(query, _resourceTypeTree);
            var resourceProxies = _resourceModification.GetResources<IPublicResource>(r => filter.Match(r as Resource)).ToArray();

            var converter = new ResourceQueryConverter(_resourceTypeTree, _serialization, query);
            return resourceProxies.Select(p => _resourceModification.Read(p.Id, r => converter.QueryConversion(r))).ToArray();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}")]
        public ActionResult<ResourceModel> GetDetails(long id)
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
            var resourceModel = _resourceModification.Read(id, r => converter.GetDetails(r));
            if (resourceModel is null)
                return NotFound($"Resource '{id}' not found!");

            return resourceModel;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}/invoke/{method}")]
        public ActionResult<Entry> InvokeMethod(long id, string method, Entry parameters)
        {
            if (_resourceModification.GetResource<IPublicResource>(id) is null)
                return NotFound($"Resource '{id}' not found!");

            Entry entry = null;
            _resourceModification.Modify(id, r =>
            {
                entry = EntryConvert.InvokeMethod(r.Descriptor, new MethodEntry { Name = method, Parameters = parameters }, _serialization);
                return true;
            });
            if (entry is null)
                return Conflict("Method could not be invoked.");

            return entry;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("types/{type}")]
        public ActionResult<ResourceModel> ConstructWithParameters(string type, string method = null, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Entry arguments = null)
        {
            var trustedType = WebUtility.HtmlEncode(type);

            return method == null ? Construct(trustedType, null)
                : Construct(trustedType, new MethodEntry { Name = method, Parameters = arguments });
        }

        private ActionResult<ResourceModel> Construct(string type, MethodEntry method)
        {
            var resource = (Resource)Activator.CreateInstance(_resourceTypeTree[type].ResourceType);
            if (resource is null)
                return NotFound();

            if (method != null)
                EntryConvert.InvokeMethod(resource, method, _serialization);

            var model = new ResourceToModelConverter(_resourceTypeTree, _serialization).GetDetails(resource);
            model.Methods = new MethodEntry[0]; // Reset methods because they can not be invoked on new objects

            return model;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        public ActionResult<ResourceModel> Save(ResourceModel model)
        {
            if (_resourceModification.GetResource<IPublicResource>(model.Id) is not null)
                return Conflict($"The resource '{model.Id}' already exists.");
            
            var id = _resourceModification.Create(_resourceTypeTree[model.Type].ResourceType, r => {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                FromModel(model, resourcesToSave, resourceCache, r);
                resourcesToSave.ForEach(id => _resourceModification.Modify(id, r => true ));
            });

            return GetDetails(id);
        }


        /// <summary>
        /// Convert ResourceModel back to resource and/or update its properties
        /// </summary>
        private Resource FromModel(ResourceModel model, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, Resource resource = null)
        {
            // Break recursion if we converted this instance already
            // Try to load by real id first
            if (cache.ContainsKey(model.Id))
                return cache[model.Id];
            // Otherwise by reference id
            if (model.Id == 0 && cache.ContainsKey(model.ReferenceId))
                return cache[model.ReferenceId];

            // Only fetch resource object if it was not given
            if (resource is null)
                if (model.Id == 0 && model.PartiallyLoaded)
                    resource = (Resource)Activator.CreateInstance(_resourceTypeTree[model.Type].ResourceType);
                else if (model.Id == 0)
                {
                    var id = _resourceModification.Create(_resourceTypeTree[model.Type].ResourceType, r => { });
                    resource = _resourceModification.Read<Resource>(model.Id, resource => resource);
                }
                else 
                    resource = _resourceModification.Read<Resource>(model.Id, resource => resource);
            
            // Write to cache because following calls might only have an empty reference
            if (model.Id == 0)
                cache[model.ReferenceId] = resource;
            else
                cache[model.Id] = resource;

            // Do not copy values from partially loaded models
            if (model.PartiallyLoaded)
                return resource;

            // Add to list if object was created or modified
            if (model.Id == 0 || model.DifferentFrom(resource, _serialization))
                resourcesToSave.Add(resource.Id);

            // Copy standard properties
            resource.Name = model.Name;
            resource.Description = model.Description;

            // Copy extended properties
            EntryConvert.UpdateInstance(resource.Descriptor, model.Properties, _serialization);

            // Set all other references
            UpdateReferences(resource, resourcesToSave, cache, model);

            return resource;
        }

        /// <summary>
        /// Updates the references of a resource
        /// </summary>
        private void UpdateReferences(Resource instance, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, ResourceModel model)
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
                            target = FromModel(targetModel, resourcesToSave, cache);
                            collection.Add(target);
                            resourcesToSave.Add(target.Id);
                        }
                        else
                        {
                            // Element already exists in the collection
                            target = (Resource)collection.First(r => r.Id == targetModel.Id);
                            FromModel(targetModel, resourcesToSave, cache, target);
                        }
                    }
                    // Remove deleted items
                    var targetIds = reference.Targets.Select(t => t.Id).Distinct().ToArray();
                    var deletedItems = collection.Where(r => !targetIds.Contains(r.Id)).ToArray();
                    foreach (var deletedItem in deletedItems)
                        collection.Remove(deletedItem);

                    if (deletedItems.Any())
                    {
                        resourcesToSave.Add(instance.Id);
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
                        target = FromModel(targetModel, resourcesToSave, cache, value);
                    else
                        target = FromModel(targetModel, resourcesToSave, cache);

                    if (target != value)
                    {
                        property.SetValue(instance, target);
                        resourcesToSave.Add(instance.Id);
                    }
                }
            }
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}")]
        public ActionResult<ResourceModel> Update(long id, ResourceModel model)
        {
            if ((_resourceModification.GetResource<IPublicResource>(id)) is null)
                return NotFound($"Resource {id} not found!");

            _resourceModification.Modify(id, r => {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                FromModel(model, resourcesToSave, resourceCache, r);
                resourcesToSave.ForEach(id => _resourceModification.Modify(id, r => true));
                return true;
            });

            return GetDetails(id);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}")]
        public ActionResult Remove(long id)
        {
            if (_resourceModification.GetResource<IPublicResource>(id) is null)
                return NotFound($"Resource {id} not found!");

            var deleted = _resourceModification.Delete(id);
            if (!deleted)
                return Conflict($"Unable to delete {id}");

            return Accepted();
        }

        private class ResourceQueryFilter
        {
            private readonly ResourceQuery _query;
            private readonly IReadOnlyList<IResourceTypeNode> _typeNodes;

            public ResourceQueryFilter(ResourceQuery query, IResourceTypeTree typeTree)
            {
                _query = query;
                _typeNodes = query.Types?.Select(typeName => typeTree[typeName]).Where(t => t != null).ToArray();
            }

            public bool Match(Resource instance)
            {
                // Check type of instance, if filter is set
                // TODO: Use type wrapper
                if (_typeNodes != null && _typeNodes.All(tn => !tn.ResourceType.IsInstanceOfType(instance)))
                    return false;

                // Next check for reference filters
                if (_query.ReferenceCondition == null)
                    return true;

                var referenceCondition = _query.ReferenceCondition;
                var references = (from property in instance.GetType().GetProperties()
                                  let att = property.GetCustomAttribute<ResourceReferenceAttribute>()
                                  where att != null
                                  select new { property, att }).ToList();

                // Find properties matching the condition
                PropertyInfo[] matches;
                if (!string.IsNullOrEmpty(referenceCondition.Name))
                {
                    matches = references.Where(r => r.property.Name == referenceCondition.Name)
                        .Select(r => r.property).ToArray();
                }
                else
                {
                    matches = references
                        .Where(r => r.att.RelationType == referenceCondition.RelationType && r.att.Role == referenceCondition.Role)
                        .Select(r => r.property).ToArray();
                }
                if (matches.Length != 1)
                    return false;

                if (referenceCondition.ValueConstraint == ReferenceValue.Irrelevant)
                    return true;

                var propertyValue = matches[0].GetValue(instance);
                if (referenceCondition.ValueConstraint == ReferenceValue.NullOrEmpty)
                    return propertyValue == null || (propertyValue as IReferenceCollection)?.UnderlyingCollection.Count == 0;

                if (referenceCondition.ValueConstraint == ReferenceValue.NotEmpty)
                    return (propertyValue as IReferenceCollection)?.UnderlyingCollection.Count > 0 || propertyValue != null;

                return true;
            }
        }
    }
}
