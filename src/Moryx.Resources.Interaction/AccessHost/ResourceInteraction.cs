// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Resources.Interaction.Converter;
using Moryx.Serialization;

namespace Moryx.Resources.Interaction
{
    /// <seealso cref="IResourceInteraction"/>
    [Plugin(LifeCycle.Singleton, typeof(IResourceInteraction))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ResourceInteraction : IResourceInteraction
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

        public string Test()
        {
            return "TestMethod";
        }

        /// <inheritdoc />
        public ResourceTypeModel GetTypeTree()
        {
            ResourceTypeModel result = null;

            try
            {
                var converter = new ResourceToModelConverter(TypeTree, Serialization);
                result = converter.ConvertType(TypeTree.RootType);
            }
            catch (Exception e)
            {
                
            }

            return result;
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
        public ResourceModel[] GetDetails(long[] ids)
        {
            var converter = new ResourceToModelConverter(TypeTree, Serialization);
            return ids.Select(Graph.Get).Select(converter.GetDetails).ToArray();
        }

        /// <inheritdoc />
        public Entry InvokeMethod(InvokeMethod invokeMethod)
        {
            var resource = Graph.Get(invokeMethod.Id);
            return EntryConvert.InvokeMethod(resource, invokeMethod.MethodModel, Serialization);
        }

        /// <inheritdoc />
        public ResourceModel Create(CreateResource createResource)
        {
            var converter = new ResourceToModelConverter(TypeTree, Serialization);

            var resource = Graph.Instantiate(createResource.ResourceType);
            if (createResource.Constructor != null)
                EntryConvert.InvokeMethod(resource, createResource.Constructor, Serialization);

            var model = converter.GetDetails(resource);
            model.Methods = new MethodEntry[0]; // Reset methods because the can not be invoked on new objects

            return model;
        }

        /// <inheritdoc />
        public ResourceModel Save(ResourceModel model)
        {
            var converter = new ModelToResourceConverter(Graph, Serialization);

            var resourcesToSave = new HashSet<Resource>();
            // Get or create resource
            converter.FromModel(model, resourcesToSave);

            // Save all created or altered resources
            foreach (var resourceToSave in resourcesToSave)
            {
                Graph.Save(resourceToSave);
            }

            return model;
        }

        /// <inheritdoc />
        public bool Remove(long id)
        {
            var resource = Graph.Get(id);
            return Graph.Destroy(resource);
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
                    select new {property, att}).ToList();

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
                if(matches.Length != 1)
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
