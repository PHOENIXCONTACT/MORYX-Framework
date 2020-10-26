// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Resources.Interaction.Converter;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Resources.Interaction
{
    /// <seealso cref="IResourceInteraction"/>
    [Plugin(LifeCycle.Singleton, typeof(IResourceInteraction))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    internal class ResourceInteraction : IResourceInteraction
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

        #endregion

        /// <inheritdoc />
        public ResourceTypeModel GetTypeTree()
        {
            var converter = new ResourceToModelConverter(TypeTree, Serialization);
            return converter.ConvertType(TypeTree.RootType);
        }

        /// <inheritdoc />
        public ResourceModel[] GetResources(ResourceQuery query)
        {
            var filter = new ResourceQueryFilter(query, TypeTree);
            var resources = Graph.GetResources<Resource>(filter.Match).ToArray();

            var converter = new ResourceQueryConverter(TypeTree, Serialization, query);
            return converter.QueryConversion(resources);
        }

        /// <inheritdoc />
        public ResourceModel GetDetails(string idString)
        {
            var id = long.Parse(idString);
            var converter = new ResourceToModelConverter(TypeTree, Serialization);
            var resource = Graph.Get(id);
            return converter.GetDetails(resource);
        }

        /// <inheritdoc />
        public ResourceModel[] GetDetailsBatch(string idString)
        {
            var ids = idString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse);
            var converter = new ResourceToModelConverter(TypeTree, Serialization);
            return ids.Select(Graph.Get).Select(converter.GetDetails).ToArray();
        }

        /// <inheritdoc />
        public Entry InvokeMethod(string idString, string name, Entry parameters)
        {
            var id = long.Parse(idString);
            var resource = Graph.Get(id);
            return EntryConvert.InvokeMethod(resource.Descriptor, new MethodEntry { Name = name, Parameters = parameters }, Serialization);
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
            var converter = new ResourceToModelConverter(TypeTree, Serialization);

            var resource = Graph.Instantiate(type);
            if (method != null)
                EntryConvert.InvokeMethod(resource, method, Serialization);

            var model = converter.GetDetails(resource);
            model.Methods = new MethodEntry[0]; // Reset methods because they can not be invoked on new objects

            return model;
        }

        /// <inheritdoc />
        public ResourceModel Save(ResourceModel model)
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
        }

        /// <inheritdoc />
        public ResourceModel Update(string idString, ResourceModel model)
        {
            var id = long.Parse(idString);
            model.Id = id;
            return Save(model);
        }

        /// <inheritdoc />
        public void Remove(string idString)
        {
            var id = long.Parse(idString);
            var resource = Graph.Get(id);
            var result = Graph.Destroy(resource);

            var context = WebOperationContext.Current;
            if (result)
                context.OutgoingResponse.StatusCode = HttpStatusCode.OK;
            else
                context.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
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
