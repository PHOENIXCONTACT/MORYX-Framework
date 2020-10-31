// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Resources.Interaction.Converter;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Resources.Interaction
{
    /// <seealso cref="IResourceInteraction"/>
    [Plugin(LifeCycle.Singleton, typeof(IResourceInteraction))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    internal class ResourceInteraction : IResourceInteraction, ILoggingComponent
    {
        #region Dependency Injection

        /// <summary>
        /// Injected by castle.
        /// </summary>
        public ICustomSerialization Serialization { get; set; }

        /// <summary>
        /// Factory to create resource instances
        /// </summary>
        public IResourceGraph Graph { get; set; }

        /// <summary>
        /// Type controller for type trees and construction
        /// </summary>
        public IResourceTypeTree TypeTree { get; set; }

        /// <summary>
        /// Logger for execution exceptions
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
        public ResourceTypeModel GetTypeTree()
        {
            return ExecuteCall<ResourceTypeModel>(delegate
            {
                var converter = new ResourceToModelConverter(TypeTree, Serialization);
                return converter.ConvertType(TypeTree.RootType);
            });
        }

        /// <inheritdoc />
        public ResourceModel[] GetResources(ResourceQuery query)
        {
            return ExecuteCall<ResourceModel[]>(delegate
            {
                var filter = new ResourceQueryFilter(query, TypeTree);
                var resources = Graph.GetResources<Resource>(filter.Match).ToArray();

                var converter = new ResourceQueryConverter(TypeTree, Serialization, query);
                return converter.QueryConversion(resources);
            });
        }

        /// <inheritdoc />
        public ResourceModel GetDetails(string idString)
        {
            return ExecuteCall(delegate
            {
                var id = long.Parse(idString);
                var converter = new ResourceToModelConverter(TypeTree, Serialization);
                var resource = Graph.Get(id);
                if (resource == null)
                    return RequestResult<ResourceModel>.NotFound($"Resource '{idString} not found!");
                return converter.GetDetails(resource);
            });
        }

        /// <inheritdoc />
        public ResourceModel[] GetDetailsBatch(string idString)
        {
            return ExecuteCall<ResourceModel[]>(delegate
            {
                var ids = idString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse);
                var converter = new ResourceToModelConverter(TypeTree, Serialization);
                return ids.Select(Graph.Get).Where(r => r != null).Select(converter.GetDetails).ToArray();
            });
        }

        /// <inheritdoc />
        public Entry InvokeMethod(string idString, string name, Entry parameters)
        {
            return ExecuteCall<Entry>(delegate
            {
                var id = long.Parse(idString);
                var resource = Graph.Get(id);
                if (resource == null)
                    return RequestResult<Entry>.NotFound($"Resource '{idString} not found!");

                return EntryConvert.InvokeMethod(resource.Descriptor, new MethodEntry {Name = name, Parameters = parameters}, Serialization);
            });
        }

        /// <inheritdoc />
        public ResourceModel Construct(string type)
        {
            return Construct(type, null);
        }

        /// <inheritdoc />
        public ResourceModel ConstructWithParameters(string type, string method, Entry arguments)
        {
            return Construct(type, new MethodEntry {Name = method, Parameters = arguments});
        }

        private ResourceModel Construct(string type, MethodEntry method)
        {
            return ExecuteCall<ResourceModel>(delegate
            {
                var converter = new ResourceToModelConverter(TypeTree, Serialization);

                var resource = Graph.Instantiate(type);
                if (method != null)
                    EntryConvert.InvokeMethod(resource, method, Serialization);

                var model = converter.GetDetails(resource);
                model.Methods = new MethodEntry[0]; // Reset methods because they can not be invoked on new objects

                return model;
            });
        }

        /// <inheritdoc />
        public ResourceModel Save(ResourceModel model)
        {
            return ExecuteCall<ResourceModel>(delegate
            {
                var converter = new ModelToResourceConverter(Graph, Serialization);

                var resourcesToSave = new HashSet<Resource>();
                // Get or create resource
                var instance = converter.FromModel(model, resourcesToSave);

                // Save all created or altered resources
                foreach (var resourceToSave in resourcesToSave)
                {
                    Graph.Save(resourceToSave);
                }

                return new ResourceToModelConverter(TypeTree, Serialization).GetDetails(instance);
            });
        }

        /// <inheritdoc />
        public ResourceModel Update(string idString, ResourceModel model)
        {
            return ExecuteCall<ResourceModel>(delegate
            {
                var resourcesToSave = new HashSet<Resource>();
                var converter = new ModelToResourceConverter(Graph, Serialization);

                Resource resource;
                if (!long.TryParse(idString, out var id) || (resource = Graph.Get(id)) == null)
                    return RequestResult<ResourceModel>.NotFound($"Resource {idString} not found!");

                // Get or create resource
                converter.FromModel(model, resourcesToSave, resource);

                // Save all created or altered resources
                foreach (var resourceToSave in resourcesToSave)
                {
                    Graph.Save(resourceToSave);
                }

                return new ResourceToModelConverter(TypeTree, Serialization).GetDetails(resource);
            });
        }

        /// <inheritdoc />
        public void Remove(string idString)
        {
            ExecuteCall(delegate
            {
                Resource resource;
                if (!long.TryParse(idString, out var id) || (resource = Graph.Get(id)) == null || !Graph.Destroy(resource))
                    return RequestResult<bool>.NotFound($"Resource {idString} not found!");
                return Graph.Destroy(resource);
            });
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

        // TODO: Duplicate between resource and product service
        private T ExecuteCall<T>(Func<RequestResult<T>> request, [CallerMemberName] string method = "Unknown")
        {
            try
            {
                var result = request();
                if (result.AlternativeStatusCode.HasValue)
                {
                    Logger.Log(LogLevel.Error, result.ErrorLog);
                    WebOperationContext.Current.OutgoingResponse.StatusCode = result.AlternativeStatusCode.Value;
                    return default;
                }
                return result.Response;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Error, ex, "Exception during '{0}'", method);
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return default;
            }
        }

        private class RequestResult<T>
        {
            public T Response { get; set; }

            public string ErrorLog { get; set; }

            public HttpStatusCode? AlternativeStatusCode { get; set; }

            public static implicit operator RequestResult<T>(T response)
            {
                return new RequestResult<T> { Response = response };
            }

            public static RequestResult<T> NotFound(string msg)
            {
                return new RequestResult<T>
                {
                    ErrorLog = msg,
                    AlternativeStatusCode = HttpStatusCode.NotFound
                };
            }
        }
    }
}
