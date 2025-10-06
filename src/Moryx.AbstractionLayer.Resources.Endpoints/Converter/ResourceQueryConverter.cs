// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    internal class ResourceQueryConverter : ResourceToModelConverter
    {
        protected ResourceQuery Query { get; }

        public ResourceQueryConverter(IResourceTypeTree typeController, ICustomSerialization serialization, ResourceQuery query) : base(typeController, serialization)
        {
            Query = query;
        }

        /// <summary>
        /// Convert resource matches of a query according to the queries reference configuration
        /// </summary>
        public ResourceModel[] QueryConversion(Resource[] matches)
        {
            var models = new ResourceModel[matches.Length];
            for (int i = 0; i < matches.Length; i++)
            {
                var model = ToModel(matches[i], true);
                model.References = Query.IncludedReferences?.Length > 0
                    ? FilteredReferences(matches[i]) : [];
                models[i] = model;
            }

            return models;
        }

        /// <summary>
        /// Convert resource match of a query according to the queries reference configuration
        /// </summary>
        public ResourceModel QueryConversion(Resource match)
        {
            var model = ToModel(match, true);
            model.References = Query.IncludedReferences?.Length > 0
                ? FilteredReferences(match) : [];
            return model;
        }

        private ResourceReferenceModel[] FilteredReferences(Resource current)
        {
            // Get references of the instance and filter them
            var node = TypeController[current.GetType().FullName];
            var references = node.References;


            var includedReferences = new List<ResourceReferenceModel>();
            foreach (var reference in references)
            {
                var filterMatched = false;

                var att = reference.GetCustomAttribute<ResourceReferenceAttribute>();
                foreach (var referenceFilter in Query.IncludedReferences)
                {
                    if (referenceFilter.Name == reference.Name)
                    {
                        filterMatched = true;
                        break;
                    }

                    if (referenceFilter.RelationType == att.RelationType && referenceFilter.Role == att.Role)
                    {
                        filterMatched = true;
                        break;
                    }
                }

                if (!filterMatched)
                    continue;

                // Convert reference and apply recursion
                var includedReference = ConvertReference(current, reference);
                includedReferences.Add(includedReference);
            }

            return includedReferences.ToArray();
        }

        protected override void ConvertReferenceRecursion(Resource resource, ResourceModel model)
        {
            if (Query.ReferenceRecursion)
                model.References = FilteredReferences(resource);
        }
    }
}
