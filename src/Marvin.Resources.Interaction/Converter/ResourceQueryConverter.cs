using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Interaction.Converter
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
                    ? FilteredReferences(matches[i]) : new ResourceReferenceModel[0];
                models[i] = model;
            }

            return models;
        }

        private ResourceReferenceModel[] FilteredReferences(Resource current)
        {
            // Get references of the instance and filter them 
            var properties = current.GetType().GetProperties();
            var references = GetReferences(properties);

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

                // TODO: This is kinda duplicate with ConvertReference in the base class. Clean this up with
                // TODO Type wrappers in AL5
                var includedReference = new ResourceReferenceModel
                {
                    Name = reference.Name,
                    Targets = new List<ResourceModel>()
                };
                var value = reference.GetValue(current);
                if (value != null)
                {
                    var collection = value as IEnumerable<IResource> ?? new[] { (IResource)value };
                    foreach (Resource resource in collection)
                    {
                        var model = ToModel(resource, true);
                        if (Query.ReferenceRecursion)
                            model.References = FilteredReferences(resource);
                        includedReference.Targets.Add(model);
                    }
                }
                includedReferences.Add(includedReference);
            }

            return includedReferences.ToArray();
        }
    }
}