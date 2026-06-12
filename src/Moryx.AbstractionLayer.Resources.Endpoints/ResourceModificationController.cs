// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moryx.AbstractionLayer.Resources.Endpoints.Models;
using Moryx.AbstractionLayer.Resources.Endpoints.Properties;
using Moryx.AspNetCore;
using Moryx.Configuration;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Tools;

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
        {
            ids = _resourceManagement.GetResources<IResource>().Select(r => r.Id).ToArray();
        }

        return ids.Select(id => _resourceManagement.ReadUnsafe(id, r => converter.GetDetails(r)))
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
        var resourceProxies = _resourceManagement.GetResourcesUnsafe<IResource>(r => filter.Match(r as Resource)).ToArray();

        var converter = new ResourceQueryConverter(_resourceTypeTree, _serialization, query);
        var values = resourceProxies.Select(p => _resourceManagement.ReadUnsafe(p.Id, r => converter.QueryConversion(r))).Where(details => details != null).ToArray();
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
        var resourceModel = _resourceManagement.ReadUnsafe(id, converter.GetDetails);
        if (resourceModel is null)
        {
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });
        }

        return resourceModel;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
    [Route("{id}/invoke/{method}")]
    [Authorize(Policy = ResourcePermissions.CanInvokeMethod)]
    public async Task<ActionResult<Entry>> InvokeMethod(long id, string method, Entry parameters)
    {
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id) is null)
        {
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });
        }

        Entry entry = null;
        try
        {
            await _resourceManagement.ModifyUnsafeAsync(id, r =>
            {
                entry = EntryConvert.InvokeMethod(r.Descriptor, new MethodEntry { Name = method, Parameters = parameters }, _serialization);
                return Task.FromResult(true);
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
    public Task<ActionResult<ResourceModel>> ConstructWithParameters(string type, string method = null, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Entry arguments = null)
    {
        var trustedType = WebUtility.HtmlEncode(type);
        if (method is null)
        {
            return Construct(trustedType);
        }

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
            {
                return BadRequest(e.Message);
            }

            throw;
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ResourcePermissions.CanAdd)]
    public async Task<ActionResult<ResourceModel>> Save(ResourceModel model)
    {
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == model.Id).Any())
        {
            return Conflict($"The resource '{model.Id}' already exists.");
        }

        try
        {
            var id = await _resourceManagement.CreateUnsafeAsync(_resourceTypeTree[model.Type].ResourceType, async (r) =>
            {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                var converter = new ModelToResourceConverter(_resourceManagement, _resourceTypeTree, _serialization);
                await converter.FromModel(model, resourcesToSave, resourceCache, r);
                foreach (var resource in resourcesToSave.Skip(1))
                {
                    await _resourceManagement.ModifyUnsafeAsync(resource, r => Task.FromResult(true));
                }
            });

            return GetDetails(id);
        }
        catch (Exception e)
        {
            if (e is ArgumentException or SerializationException or ValidationException)
            {
                return BadRequest(e.Message);
            }

            throw;
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
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id) is null)
        {
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });
        }

        try
        {
            _resourceManagement.ModifyUnsafeAsync(id, async (r) =>
            {
                var resourcesToSave = new HashSet<long>();
                var resourceCache = new Dictionary<long, Resource>();
                var converter = new ModelToResourceConverter(_resourceManagement, _resourceTypeTree, _serialization);
                await converter.FromModel(model, resourcesToSave, resourceCache, r);
                resourcesToSave.ForEach(id => _resourceManagement.ModifyUnsafeAsync(id, _ => Task.FromResult(true)));
                return true;
            });
        }
        catch (Exception e)
        {
            if (e is ArgumentException or SerializationException or ValidationException)
            {
                return BadRequest(e.Message);
            }

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
    public async Task<ActionResult> Remove(long id)
    {
        if (_resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == id) is null)
        {
            return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ResourceNotFoundException_ById_Message, id) });
        }

        var deleted = await _resourceManagement.DeleteAsync(id);
        if (!deleted)
        {
            return Conflict($"Unable to delete {id}");
        }

        return Accepted();
    }

    private sealed class ResourceQueryFilter
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
            {
                return false;
            }

            // Next check for reference filters
            if (_query.ReferenceCondition == null)
            {
                return true;
            }

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
            {
                return false;
            }

            if (referenceCondition.ValueConstraint == ReferenceValue.Irrelevant)
            {
                return true;
            }

            var propertyValue = matches[0].GetValue(instance);
            if (referenceCondition.ValueConstraint == ReferenceValue.NullOrEmpty)
            {
                return propertyValue == null || (propertyValue as IReferenceCollection)?.UnderlyingCollection.Count == 0;
            }

            if (referenceCondition.ValueConstraint == ReferenceValue.NotEmpty)
            {
                return (propertyValue as IReferenceCollection)?.UnderlyingCollection.Count > 0 || propertyValue != null;
            }

            return true;
        }
    }
}
