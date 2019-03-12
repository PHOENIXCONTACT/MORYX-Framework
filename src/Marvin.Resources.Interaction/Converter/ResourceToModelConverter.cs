using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Interaction.Converter
{
    /// <summary>
    /// Converts Resources to ResourceModel
    /// </summary>
    internal class ResourceToModelConverter
    {
        /// <summary>
        /// Type cache to avoid redundant conversions AND make use of WCFs "IsReference" feature
        /// </summary>
        private static readonly Dictionary<string, ResourceTypeModel> TypeCache = new Dictionary<string, ResourceTypeModel>();

        /// <summary>
        /// Resource cache to avoid redundant conversions AND make use of WCFs "IsReference" feature
        /// </summary>
        private readonly Dictionary<Resource, ResourceModel> _resourceCache = new Dictionary<Resource, ResourceModel>();

        private readonly IResourceGraph _resourceGraph;
        private readonly ICustomSerialization _serialization;
        private readonly IResourceTypeTree _typeController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceGraph"></param>
        /// <param name="typeController"></param>
        /// <param name="serialization"></param>
        public ResourceToModelConverter(IResourceGraph resourceGraph, IResourceTypeTree typeController, ICustomSerialization serialization)
        {
            _resourceGraph = resourceGraph;
            _typeController = typeController;
            _serialization = serialization;
        }

        /// <summary>
        /// Recursive function to load resource and its children
        /// </summary>
        /// <param name="current">Current instance</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public ResourceModel GetDetails(Resource current, int depth)
        {
            ResourceModel model;
            if (!_resourceCache.TryGetValue(current, out model))
            {
                // Extract model
                model = new ResourceModel
                {
                    Id = current.Id,
                    Name = current.Name,
                    Description = current.Description,

                    // Use simplified type reference
                    Type = current.ResourceType(),

                    // Properties and methods are read from the descriptor
                    // This can be the resource itself or a dedicated object
                    Properties = EntryConvert.EncodeObject(current.Descriptor, _serialization),
                    Methods = EntryConvert.EncodeMethods(current.Descriptor, _serialization).ToArray()
                };
                _resourceCache.Add(current, model);

                // Recursively read children and references
                model.References = ConvertReferences(current, depth);
            }

            return model;
        }

        /// <summary>
        /// Convert all references of a resource
        /// </summary>
        private ResourceReferenceModel[] ConvertReferences(Resource current, int depth)
        {
            var properties = current.GetType().GetProperties();
            // Find all reference properties on the object
            var referenceProperties = GetReferences(properties);
            // Find all properties that define a type override of existing references
            var referenceOverrides = GetReferenceOverrides(properties);

            // Convert all references to DTOs
            return referenceProperties.Select(prop => ConvertReference(current, prop, referenceOverrides, depth)).ToArray();
        }

        private ResourceReferenceModel[] ChildrenOnly(Resource current)
        {
            // Get the children reference
            var childrenProperty = current.GetType().GetProperty(nameof(Resource.Children));
            // Find possible overrides on the children reference
            var referenceOverrides = GetReferenceOverrides(current.GetType().GetProperties());

            var model = ConvertReference(current, childrenProperty, referenceOverrides);
            model.Targets = current.Children.Select(ConvertResource).ToList();

            return new[] { model };
        }

        public ResourceModel ConvertResource(Resource resource)
        {
            return new ResourceModel
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                Type = resource.ResourceType(),
                References = ChildrenOnly(resource)
            };
        }

        #region Convert References

        /// <summary>
        /// Get all references 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetReferences(IEnumerable<PropertyInfo> properties)
        {
            var referenceProperties = (from prop in properties
                                       let propType = prop.PropertyType
                                       // Find all properties referencing a resource or a collection of resources
                                        // Exclude read only properties, because they are simple type overrides of other references
                                       where prop.CanWrite && Attribute.IsDefined(prop, typeof(ResourceReferenceAttribute))
                                       select prop).ToList();
            return referenceProperties;
        }


        /// <summary>
        /// Get all reference overrides from a resources properties
        /// </summary>
        private static Dictionary<string, List<Type>> GetReferenceOverrides(IEnumerable<PropertyInfo> properties)
        {
            var referenceOverrides = (from prop in properties
                                      let overrideAtt = prop.GetCustomAttribute<ReferenceOverrideAttribute>()
                                      where overrideAtt != null
                                      let targetType = typeof(IEnumerable<IResource>).IsAssignableFrom(prop.PropertyType)
                                        ? EntryConvert.ElementType(prop.PropertyType) : prop.PropertyType
                                      group targetType by overrideAtt.Source into g
                                      select new { g.Key, overrides = g.ToList() }).ToDictionary(v => v.Key, v => v.overrides);
            return referenceOverrides;
        }

        /// <summary>
        /// Convert a property referencing another resource into a <see cref="ResourceReferenceModel"/>
        /// </summary>
        private ResourceReferenceModel ConvertReference(Resource current, PropertyInfo property, IDictionary<string, List<Type>> overrides, int depth = 0)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();

            // Create reference model from property information and optional attribute
            var referenceModel = new ResourceReferenceModel
            {
                Name = property.Name,
                Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description,

                Role = attribute.Role,
                RelationType = attribute.RelationType,
                IsCollection = typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType)
            };

            // Get type constraints
            Type targetType = property.PropertyType;
            if (referenceModel.IsCollection)
                targetType = EntryConvert.ElementType(targetType);
            var typeConstraints = MergeTypeConstraints(property, targetType, overrides);
            referenceModel.SupportedTypes = SupportedTypes(typeConstraints);

            // Only load possible targets if we are supposed to
            if (depth <= 0)
                return referenceModel;

            // Load possible targets from the full set of resources
            referenceModel.PossibleTargets = MatchingInstances(typeConstraints).ToList();

            // We can not load targets if we do not have any
            referenceModel.Targets = new List<ResourceModel>();
            var value = property.GetValue(current);
            if (value == null)
                return referenceModel;

            // Convert referenced resource objects and possible instance types 
            var referenceTargets = referenceModel.IsCollection ? (IEnumerable<IResource>)value : new[] { (IResource)value };
            foreach (Resource resource in referenceTargets)
            {
                var target = GetDetails(resource, depth - 1);
                referenceModel.Targets.Add(target);
                // Possible targets must always include the current target, even if its not part of the tree
                if (referenceModel.PossibleTargets.All(pt => pt.Id != target.Id))
                    referenceModel.PossibleTargets.Add(target);
            }

            return referenceModel;
        }

        /// <summary>
        /// Determine all type constraints for this reference property. Type constraints are created by the usage
        /// of <see cref="ReferenceOverrideAttribute"/> on readonly properties to limit allowed types for the referenced property
        /// </summary>
        private static ICollection<Type> MergeTypeConstraints(PropertyInfo property, Type targetType, IDictionary<string, List<Type>> referenceOverrides)
        {
            // If there are no overrides the only limitation is the target type
            if (!referenceOverrides.ContainsKey(property.Name))
                return new[] { targetType };

            // Otherwise find all types that limit the reference type without redundancies. This means eliminating all types
            // represented by another type
            var myOverrides = referenceOverrides[property.Name];
            return myOverrides.Concat(new[] { targetType })
                .Where(type => !myOverrides.Any(over => over != type && type.IsAssignableFrom(over))).ToList();
        }

        /// <summary>
        /// Convert the supported types 
        /// </summary>
        private ResourceTypeModel[] SupportedTypes(ICollection<Type> typeConstraints)
        {
            return _typeController.SupportedTypes(typeConstraints).Select(t => ConvertType(t, _serialization)).ToArray();
        }

        /// <summary>
        /// Find all resources of the tree that are possible instances for a reference of that type
        /// </summary>
        private IEnumerable<ResourceModel> MatchingInstances(ICollection<Type> typeConstraints)
        {
            var matches = new List<Resource>();

            foreach (var root in _resourceGraph.GetResources<Resource>(r => r.Parent == null))
            {
                IncludeMatchingInstance(root, typeConstraints, matches);
            }

            return matches.Select(r => new ResourceModel
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.ResourceType(),
                Description = r.Description
            });
        }

        /// <summary>
        /// Include this resource into the collection of matches if it is an instance of the desired type
        /// </summary>
        private static void IncludeMatchingInstance(Resource current, ICollection<Type> typeConstraints, ICollection<Resource> currentMatches)
        {
            if (typeConstraints.All(tc => tc.IsInstanceOfType(current)))
                currentMatches.Add(current);

            foreach (var child in current.Children)
            {
                IncludeMatchingInstance(child, typeConstraints, currentMatches);
            }
        }

        /// <summary>
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        internal static ResourceTypeModel ConvertType(IResourceTypeNode node, ICustomSerialization serialization)
        {
            return ConvertType(node, null, serialization);
        }

        /// <summary>
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        internal static ResourceTypeModel ConvertType(IResourceTypeNode node, ResourceTypeModel baseType, ICustomSerialization serialization)
        {
            var resType = node.ResourceType;

            if (TypeCache.ContainsKey(node.Name))
                return TypeCache[node.Name];

            // Remove generic arguments from type name
            var typeModel = new ResourceTypeModel
            {
                Creatable = node.Creatable,
                Name = node.Name,
                BaseType = baseType,

                // Read display name of the type otherwise use type short name
                DisplayName = resType.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName ??
                              Regex.Replace(resType.Name, @"`\d", string.Empty),

                // Read description of the type
                Description = resType.GetCustomAttribute<DescriptionAttribute>(false)?.Description,

                // Convert resource constructors
                Constructors = node.Constructors.Select(ctr => EntryConvert.EncodeMethod(ctr, serialization)).ToArray()
            };

            typeModel.DerivedTypes = node.DerivedTypes.Select(t => ConvertType(t, typeModel, serialization)).ToArray();

            TypeCache[node.Name] = typeModel;

            return typeModel;
        }

        #endregion
    }
}
