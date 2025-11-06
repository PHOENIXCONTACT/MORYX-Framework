// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moryx.Asp.Extensions;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Runtime.Modules;
using Moryx.Configuration;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Resources.Endpoints.Properties;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IResourceManagement"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/resources/")]
    [Produces("application/json")]
    public class ResourceModificationController : ControllerBase
    {
        private readonly IResourceManagement _resourceManagement;
        private readonly IResourceTypeTree _resourceTypeTree;
        private readonly ResourceSerialization _serialization;

        public ResourceModificationController(IResourceManagement resourceManagement,
            IResourceTypeTree resourceTypeTree,
            IModuleManager moduleManager,
            IServiceProvider serviceProvider)
        {
            _resourceManagement = resourceManagement ?? throw new ArgumentNullException(nameof(resourceManagement));
            _resourceTypeTree = resourceTypeTree ?? throw new ArgumentNullException(nameof(resourceTypeTree));
            var module = moduleManager.AllModules.FirstOrDefault(module => module is IFacadeContainer<IResourceManagement>);
            _serialization = new ResourceSerialization(module.Container, serviceProvider);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("types")]
        [Authorize(Policy = ResourcePermissions.CanViewTypeTree)]
        public ActionResult<ResourceTypeModel> GetTypeTree()
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
            return converter.ConvertType(_resourceTypeTree.RootType);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Authorize(Policy = ResourcePermissions.CanViewDetails)]
        public ActionResult<ResourceModel[]> GetDetailsBatch([FromQuery] long[] ids)
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);

            if (ids is null || ids.Length == 0)
                ids = _resourceManagement.GetResources<IResource>().Select(r => r.Id).ToArray();

            return ids.Select(id => _resourceManagement.Read(id, r => converter.GetDetails(r)))
                .Where(details => details != null).ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("query")]
        [Authorize(Policy = ResourcePermissions.CanViewTree)]
        public ActionResult<ResourceModel[]> GetResources(ResourceQuery query)
        {
            var filter = new ResourceQueryFilter(query, _resourceTypeTree);
            var resourceProxies = _resourceManagement.GetAllResources<IResource>(r => filter.Match(r as Resource)).ToArray();

            var converter = new ResourceQueryConverter(_resourceTypeTree, _serialization, query);
            var values = resourceProxies.Select(p => _resourceManagement.Read(p.Id, r => converter.QueryConversion(r))).Where(details => details != null).ToArray();
            return values;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}")]
        [Authorize(Policy = ResourcePermissions.CanViewDetails)]
        public ActionResult<ResourceModel> GetDetails(long id)
        {
            var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
            var resourceModel = _resourceManagement.Read(id, r => converter.GetDetails(r));
            if (resourceModel is null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

            return resourceModel;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}/invoke/{method}")]
        [Authorize(Policy = ResourcePermissions.CanInvokeMethod)]
        public ActionResult<Entry> InvokeMethod(long id, string method, Entry parameters)
        {
            if (_resourceManagement.GetAllResources<IResource>(r => r.Id == id) is null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

            Entry entry = null;
            try
            {
                _resourceManagement.Modify(id, r =>
                {
                    entry = EntryConvert.InvokeMethod(r.Descriptor, new MethodEntry { Name = method, Parameters = parameters }, _serialization);
                    return true;
                });
            }
            catch (MissingMethodException)
            {
                return BadRequest("Method could not be invoked. Please check spelling and access modifier (has to be `public` or `internal`).");
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return entry;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("types/{type}")]
        [Authorize(Policy = ResourcePermissions.CanAdd)]
        public ActionResult<ResourceModel> ConstructWithParameters(string type, string method = null, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Entry arguments = null)
        {
            var trustedType = WebUtility.HtmlEncode(type);
            if (method is null)
                return Construct(trustedType);
            else
                return Construct(trustedType, new MethodEntry { Name = method, Parameters = arguments });
        }

        private ActionResult<ResourceModel> Construct(string type)
        {
            Resource resource;
            try
            {
                resource = (Resource)Activator.CreateInstance(_resourceTypeTree[type].ResourceType);
            }
            catch (Exception)
            {
                return NotFound(new MoryxExceptionResponse { Title = Strings.ResourceManagementController_ResourceNotFound });
            }

            ValueProviderExecutor.Execute(resource, new ValueProviderExecutorSettings()
                .AddFilter(new DataMemberAttributeValueProviderFilter(false))
                .AddDefaultValueProvider());

            var model = new ResourceToModelConverter(_resourceTypeTree, _serialization).GetDetails(resource);
            model.Methods = []; // Reset methods because they can not be invoked on new objects
            return model;
        }

        private ActionResult<ResourceModel> Construct(string type, MethodEntry method)
        {
            try
            {
                var id = _resourceManagement.Create(_resourceTypeTree[type].ResourceType, r => EntryConvert.InvokeMethod(r, method, _serialization));
                return GetDetails(id);
            }
            catch (Exception e)
            {
                if (e is ArgumentException or SerializationException or ValidationException)
                    return BadRequest(e.Message);
                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = ResourcePermissions.CanAdd)]
        public ActionResult<ResourceModel> Save(ResourceModel model)
        {
            if (_resourceManagement.GetAllResources<IResource>(r => r.Id == model.Id).Any())
                return Conflict($"The resource '{model.Id}' already exists.");
            try
            {
                var id = _resourceManagement.Create(_resourceTypeTree[model.Type].ResourceType, r =>
                {
                    var resourcesToSave = new HashSet<long>();
                    var resourceCache = new Dictionary<long, Resource>();
                    FromModel(model, resourcesToSave, resourceCache, r);
                    resourcesToSave.Skip(1).ForEach(id => _resourceManagement.Modify(id, r => true));
                });

                return GetDetails(id);
            }
            catch (Exception e)
            {
                if (e is ArgumentException or SerializationException or ValidationException)
                    return BadRequest(e.Message);
                throw;
            }
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
                    var id = _resourceManagement.Create(_resourceTypeTree[model.Type].ResourceType, r => { });
                    resource = _resourceManagement.Read<Resource>(id, resource => resource);
                }
                else
                    resource = _resourceManagement.Read<Resource>(model.Id, resource => resource);

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        [Authorize(Policy = ResourcePermissions.CanEdit)]
        public ActionResult<ResourceModel> Update(long id, ResourceModel model)
        {
            if (_resourceManagement.GetAllResources<IResource>(r => r.Id == id) is null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

            try
            {
                _resourceManagement.Modify(id, r =>
                {
                    var resourcesToSave = new HashSet<long>();
                    var resourceCache = new Dictionary<long, Resource>();
                    FromModel(model, resourcesToSave, resourceCache, r);
                    resourcesToSave.ForEach(id => _resourceManagement.Modify(id, r => true));
                    return true;
                });
            }
            catch (Exception e)
            {
                if (e is ArgumentException or SerializationException or ValidationException)
                    return BadRequest(e.Message);
                throw;
            }

            return GetDetails(id);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [Route("{id}")]
        [Authorize(Policy = ResourcePermissions.CanDelete)]
        public ActionResult Remove(long id)
        {
            if (_resourceManagement.GetAllResources<IResource>(r => r.Id == id) is null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

            var deleted = _resourceManagement.Delete(id);
            if (!deleted)
                return Conflict($"Unable to delete {id}");

            return Accepted();
        }

        private class ResourceQueryFilter
        {
            private readonly ResourceQuery _query;
            private readonly IReadOnlyList<IResourceTypeNode> _typeNodes;
            private readonly IResourceTypeTree _resourceTypeTree;

            public ResourceQueryFilter(ResourceQuery query, IResourceTypeTree typeTree)
            {
                _query = query;
                _typeNodes = query.Types?.Select(typeName => typeTree[typeName]).Where(t => t != null).ToArray();
                _resourceTypeTree = typeTree;
            }

            public bool Match(Resource instance)
            {
                // Check type of instance, if filter is set
                if (_typeNodes != null && _typeNodes.All(tn => !tn.ResourceType.IsInstanceOfType(instance)))
                    return false;

                // Next check for reference filters
                if (_query.ReferenceCondition == null)
                    return true;

                var node = _resourceTypeTree[instance.GetType().FullName];

                var referenceCondition = _query.ReferenceCondition;
                var references = (from property in node.PropertiesOfResourceType
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
