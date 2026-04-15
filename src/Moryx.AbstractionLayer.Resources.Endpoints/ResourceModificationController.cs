// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Runtime.Modules;
using Moryx.Configuration;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Resources.Endpoints.Models;
using Moryx.AbstractionLayer.Resources.Endpoints.Properties;
using Moryx.AspNetCore;

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IResourceManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/resources/")]
[Produces("application/json")]
//TODO: Rename to ResourceManagementController in next major version
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

    /// <summary>
    /// Returns the full resource type tree
    /// </summary>
    /// <returns>The root node of the resource type tree.</returns>
    [HttpGet("types")]
    [ProducesResponseType(typeof(ResourceTypeModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanViewTypeTree)]
    public ActionResult<ResourceTypeModel> GetTypeTree()
    {
        var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
        return converter.ConvertType(_resourceTypeTree.RootType);
    }

    /// <summary>
    /// Returns the details for one or more resources by their IDs.
    /// </summary>
    /// <param name="ids">
    /// One or more resource IDs.
    /// When left empty, details for all resources are returned.
    /// </param>
    /// <returns> An array of resource detail models.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ResourceModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanViewDetails)]
    public ActionResult<ResourceModel[]> GetDetailsBatch([FromQuery] long[] ids)
    {
        var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);

        if (ids is null || ids.Length == 0)
            ids = _resourceManagement.GetResources<IResource>().Select(r => r.Id).ToArray();

        return ids.Select(id => _resourceManagement.ReadUnsafe(id, r => converter.GetDetails(r)))
            .Where(details => details != null).ToArray();
    }

    /// <summary>
    /// Returns all resources matching the specified query filter.
    /// All query parameters are optional. When none are specified all resources are returned.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("query")]
    [ProducesResponseType(typeof(ResourceModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanViewTree)]
    public ActionResult<ResourceModel[]> GetResources([FromQuery] ResourceQuery query)
    {
        var filter = new ResourceQueryFilter(query, _resourceTypeTree);
        var resourceProxies = _resourceManagement.GetResourcesUnsafe<IResource>(r => filter.Match(r as Resource)).ToArray();

        var converter = new ResourceQueryConverter(_resourceTypeTree, _serialization, query);
        var values = resourceProxies.Select(p => _resourceManagement.ReadUnsafe(p.Id, r => converter.QueryConversion(r))).Where(details => details != null).ToArray();
        return values;
    }

    /// <summary>
    /// Returns the details of a resource by its ID.
    /// </summary>
    /// <param name="id"> The ID of the resource.</param>
    /// <returns>The full model of the requested resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResourceModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanViewDetails)]
    public ActionResult<ResourceModel> GetDetails(long id)
    {
        var converter = new ResourceToModelConverter(_resourceTypeTree, _serialization);
        var resourceModel = _resourceManagement.ReadUnsafe(id, r => converter.GetDetails(r));
        if (resourceModel is null)
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

        return resourceModel;
    }

    /// <summary>
    /// Invokes a named action on the resource.
    /// Available actions are listed in the 'methods' field of the resource returned by GET /{id}.
    /// </summary>
    /// <param name="id">The ID of the resource.</param>
    /// <param name="action">The name of the action to invoke.</param>
    /// <param name="parameters">
    /// The action's input parameters structured as an Entry tree.
    /// Pass an empty body if the action takes no parameters.
    /// </param>
    /// <returns>
    /// The action's return value as an Entry tree (200 OK),
    /// or an empty body when the action returns void (204 No Content).
    /// </returns>
    [HttpPost("{id}/actions/{action}")]
    [ProducesResponseType(typeof(Entry), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Policy = ResourcePermissions.CanInvokeMethod)]
    public async Task<ActionResult<Entry>> InvokeAction(long id, string action, Entry parameters)
    {
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id) is null)
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

        Entry entry = null;
        try
        {
            await _resourceManagement.ModifyUnsafeAsync(id, r =>
            {
                entry = EntryConvert.InvokeMethod(r.Descriptor, new MethodEntry { Name = action, Parameters = parameters }, _serialization);
                return Task.FromResult(true);
            });
        }
        catch (MissingMethodException)
        {
            return NotFound(new MoryxExceptionResponse { Title = $"Action '{action}' does not exist on resource {id}." });
        }
        catch (Exception e)
        {
            return UnprocessableEntity(new MoryxExceptionResponse { Title = $"Action '{action}' failed: {e.Message}." });
        }

        return entry is null ? NoContent() : Ok(entry);
    }

    /// <summary>
    /// Constructs a new resource instance of the specified type without persisting it.
    /// Optionally invokes a constructor method with the supplied arguments, which does persist it.
    /// </summary>
    /// <param name="type">
    /// The resource type name to construct (e.g. MyNamespace.MyResource).
    /// Available types are returned by GET /types.
    /// </param>
    /// <param name="method">
    /// The name of an optional constructor method to invoke.
    /// Available constructor names are listed in the methods field of the unpersisted instance
    /// returned when this parameter is omitted.
    /// </param>
    /// <param name="arguments">
    /// Arguments for the constructor method, structured as an Entry tree.
    /// Pass an empty body if the constructor takes no arguments.
    /// </param>
    /// <returns>
    /// The constructed resource model. The Id is 0 when no constructor was invoked,
    /// otherwise the assigned ID is included.
    /// </returns>
    [HttpPost("types/{type}")]
    [ProducesResponseType(typeof(ResourceModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanAdd)]
    public Task<ActionResult<ResourceModel>> ConstructWithParameters(string type, string method = null, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Entry arguments = null)
    {
        var trustedType = WebUtility.HtmlEncode(type);
        if (method is null)
            return Construct(trustedType);

        return Construct(trustedType, new MethodEntry { Name = method, Parameters = arguments });
    }

    private async Task<ActionResult<ResourceModel>> Construct(string type)
    {
        Resource resource;
        try
        {
            resource = (Resource)Activator.CreateInstance(_resourceTypeTree[type].ResourceType);
        }
        catch (Exception)
        {
            return NotFound(new MoryxExceptionResponse
            {
                Title = Strings.ResourceManagementController_ResourceNotFound
            });
        }

        ValueProviderExecutor.Execute(resource, new ValueProviderExecutorSettings()
            .AddFilter(new DataMemberAttributeValueProviderFilter(false))
            .AddDefaultValueProvider());

        var model = new ResourceToModelConverter(_resourceTypeTree, _serialization).GetDetails(resource);
        model.Methods = []; // Reset methods because they can not be invoked on new objects
        return model;
    }

    private async Task<ActionResult<ResourceModel>> Construct(string type, MethodEntry method)
    {
        try
        {
            var id = await _resourceManagement.CreateUnsafeAsync(_resourceTypeTree[type].ResourceType, resource =>
            {
                EntryConvert.InvokeMethod(resource, method, _serialization);
                return Task.CompletedTask;
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
    /// Saves a new resource to the database.
    /// Returns the saved resource with its assigned ID.
    /// </summary>
    /// <param name="model">
    /// The resource model to persist. The Id field must be 0.
    /// Nested references with Id = 0 are created; references with an existing ID are linked.
    /// </param>
    /// <returns>The saved resource model with its assigned ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResourceModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanAdd)]
    public async Task<ActionResult<ResourceModel>> Save(ResourceModel model)
    {
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == model.Id).Any())
            return Conflict(new MoryxExceptionResponse { Title = $"Resource '{model.Id}' already exists. Use PUT /{model.Id} to update it." });
        try
        {
            var id = await _resourceManagement.CreateUnsafeAsync(_resourceTypeTree[model.Type].ResourceType, async (r) =>
            {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                await FromModel(model, resourcesToSave, resourceCache, r);
                foreach (var resource in resourcesToSave.Skip(1))
                {
                    await _resourceManagement.ModifyUnsafeAsync(resource, r => Task.FromResult(true));
                }
            });

            var created = GetDetails(id);
            return CreatedAtAction(nameof(GetDetails), new { id }, created.Value);
        }
        catch (Exception e)
        {
            if (e is ArgumentException or SerializationException or ValidationException)
                return BadRequest(new MoryxExceptionResponse { Title = e.Message });
            throw;
        }
    }

    /// <summary>
    /// Convert ResourceModel back to resource and/or update its properties
    /// </summary>
    private async Task<Resource> FromModel(ResourceModel model, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, Resource resource = null)
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
                var id = await _resourceManagement.CreateUnsafeAsync(_resourceTypeTree[model.Type].ResourceType, r => Task.CompletedTask);
                resource = _resourceManagement.ReadUnsafe(id, r => r);
            }
            else
                resource = _resourceManagement.ReadUnsafe(model.Id, r => r);

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
        await UpdateReferences(resource, resourcesToSave, cache, model);

        return resource;
    }

    /// <summary>
    /// Updates the references of a resource
    /// </summary>
    private async Task UpdateReferences(Resource instance, HashSet<long> resourcesToSave, Dictionary<long, Resource> cache, ResourceModel model)
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
                        target = await FromModel(targetModel, resourcesToSave, cache);
                        collection.Add(target);
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
                    target = await FromModel(targetModel, resourcesToSave, cache, value);
                else
                    target = await FromModel(targetModel, resourcesToSave, cache);

                if (target != value)
                {
                    property.SetValue(instance, target);
                    resourcesToSave.Add(instance.Id);
                }
            }
        }
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="id">The ID of the resource to update.</param>
    /// <param name="model">
    /// The updated resource model. The model's type must match the existing resource's type.
    /// </param>
    /// <returns>The resource model as it exists in the databse after the update.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResourceModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanEdit)]
    public async Task<ActionResult<ResourceModel>> Update(long id, ResourceModel model)
    {
        if (!_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id).Any())
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

        try
        {
            await _resourceManagement.ModifyUnsafeAsync(id, async (r) =>
            {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                await FromModel(model, resourcesToSave, resourceCache, r);
                foreach (var resourceId in resourcesToSave.Skip(1))
                {
                    await _resourceManagement.ModifyUnsafeAsync(resourceId, _ => Task.FromResult(true));
                }
                return true;
            });
        }
        catch (Exception e)
        {
            if (e is ArgumentException or SerializationException or ValidationException)
                return BadRequest(new MoryxExceptionResponse { Title = e.Message });
            throw;
        }

        return GetDetails(id);
    }

    /// <summary>
    /// Deletes the resource with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the resource to delete.</param>
    /// <returns>
    /// No content on success.
    /// Returns conflict if the resource cannot be deleted because it is still referenced by other resources.
    /// </returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanDelete)]
    public async Task<ActionResult> Remove(long id)
    {
        if (!_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id).Any())
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });

        var deleted = await _resourceManagement.DeleteAsync(id);
        if (!deleted)
            return Conflict(new MoryxExceptionResponse { Title = $"Resource {id} cannot be deleted while it is still referenced by other resources."});

        return NoContent();
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
