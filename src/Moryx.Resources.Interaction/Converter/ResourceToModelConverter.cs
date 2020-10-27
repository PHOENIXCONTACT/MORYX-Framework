// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Resources.Interaction.Converter
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
        /// Internal ref id sequence
        /// </summary>
        private long _refId = 0;

        /// <summary>
        /// Resource cache to avoid redundant conversions AND make use of WCFs "IsReference" feature
        /// </summary>
        private readonly Dictionary<Resource, long> _resourceCache = new Dictionary<Resource, long>();

        protected ICustomSerialization Serialization { get; }

        protected IResourceTypeTree TypeController { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeController"></param>
        /// <param name="serialization"></param>
        public ResourceToModelConverter(IResourceTypeTree typeController, ICustomSerialization serialization)
        {
            TypeController = typeController;
            Serialization = serialization;
        }

        /// <summary>
        /// Convert a resource instance
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public ResourceModel GetDetails(Resource current)
        {
            return ToModel(current, false);
        }

        /// <summary>
        /// Recursive function to load resource and its children
        /// </summary>
        /// <param name="current">Current instance</param>
        /// <param name="partially">Model does not represent the full resource</param>
        /// <returns></returns>
        protected ResourceModel ToModel(Resource current, bool partially)
        {
            ResourceModel model;
            if (_resourceCache.ContainsKey(current))
            {
                // Include reference instead
                model = new ResourceModel { ReferenceId = _resourceCache[current] };
            }
            else
            {
                // Extract model
                model = new ResourceModel
                {
                    Id = current.Id,
                    ReferenceId = ++_refId,

                    Name = current.Name,
                    Description = current.Description,

                    // Use simplified type reference
                    Type = current.ResourceType()
                };
                _resourceCache.Add(current, model.ReferenceId);

                // Set partial flag or load complex properties depending on details depth
                if (partially)
                {
                    model.PartiallyLoaded = true;
                }
                else
                {
                    // Properties and methods are read from the descriptor
                    // This can be the resource itself or a dedicated object
                    model.Properties = EntryConvert.EncodeObject(current.Descriptor, Serialization);
                    model.Methods = EntryConvert.EncodeMethods(current.Descriptor, Serialization).ToArray();
                    // Recursively read children and references
                    model.References = ConvertReferences(current);
                }
            }

            return model;
        }

        /// <summary>
        /// Convert all references of a resource
        /// </summary>
        private ResourceReferenceModel[] ConvertReferences(Resource current)
        {
            var properties = current.GetType().GetProperties();
            // Find all reference properties on the object
            var referenceProperties = GetReferences(properties);

            // Convert all references to DTOs
            return referenceProperties.Select(prop => ConvertReference(current, prop)).ToArray();
        }

        #region Convert References

        /// <summary>
        /// Get all reference properties
        /// </summary>
        protected static IEnumerable<PropertyInfo> GetReferences(IEnumerable<PropertyInfo> properties)
        {
            // TODO Type wrappers in AL5
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
            // TODO Type wrappers in AL5
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
        protected ResourceReferenceModel ConvertReference(Resource current, PropertyInfo property)
        {
            // Create reference model from property information and optional attribute
            var referenceModel = new ResourceReferenceModel
            {
                Name = property.Name
            };

            // We can not set current targets if we do not have any
            var value = property.GetValue(current);
            if (value == null)
                return referenceModel;

            // Convert referenced resource objects and possible instance types
            // TODO Type wrappers in AL5
            var referenceTargets = (value as IEnumerable<IResource>) ?? new[] { (IResource)value };
            foreach (Resource resource in referenceTargets)
            {
                // Load references partially UNLESS they are new, unsaved objects
                var model = ToModel(resource, resource.Id > 0);
                ConvertReferenceRecursion(resource, model);
                referenceModel.Targets.Add(model);
            }

            return referenceModel;
        }

        /// <summary>
        /// Optional recursion for resources during reference conversion
        /// </summary>
        protected virtual void ConvertReferenceRecursion(Resource resource, ResourceModel model)
        {
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
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        internal ResourceTypeModel ConvertType(IResourceTypeNode node)
        {
            return ConvertType(node, null);
        }

        /// <summary>
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        internal ResourceTypeModel ConvertType(IResourceTypeNode node, ResourceTypeModel baseType)
        {
            var resType = node.ResourceType;

            if (TypeCache.ContainsKey(node.Name))
                return TypeCache[node.Name];

            // Remove generic arguments from type name
            var typeModel = new ResourceTypeModel
            {
                Creatable = node.Creatable,
                Name = node.Name,
                BaseType = baseType?.Name,

                // Read display name of the type otherwise use type short name
                DisplayName = resType.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName ??
                              Regex.Replace(resType.Name, @"`\d", string.Empty),

                // Read description of the type
                Description = resType.GetCustomAttribute<DescriptionAttribute>(false)?.Description,

                // Convert resource constructors
                Constructors = node.Constructors.Select(ctr => EntryConvert.EncodeMethod(ctr, Serialization)).ToArray()
            };

            // Convert reference properties
            var properties = node.ResourceType.GetProperties();
            var references = GetReferences(properties);
            var overrides = GetReferenceOverrides(properties);
            typeModel.References = references.Select(reference => ConvertReferenceProperty(reference, overrides)).ToArray();

            typeModel.DerivedTypes = node.DerivedTypes.Select(t => ConvertType(t, typeModel)).ToArray();

            TypeCache[node.Name] = typeModel;

            return typeModel;
        }

        private ReferenceTypeModel ConvertReferenceProperty(PropertyInfo property, IDictionary<string, List<Type>> overrides)
        {
            var referenceAttr = property.GetCustomAttribute<ResourceReferenceAttribute>();
            var displayName = property.GetDisplayName();

            // Create reference model from property information and optional attribute
            var referenceModel = new ReferenceTypeModel
            {
                Name = property.Name,
                DisplayName = !string.IsNullOrEmpty(displayName) ? displayName : property.Name,
                Description = property.GetDescription(),
                Role = referenceAttr.Role,
                RelationType = referenceAttr.RelationType,
                IsRequired = referenceAttr.IsRequired,
                IsCollection = typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType)
            };

            // Get type constraints
            Type targetType = property.PropertyType;
            if (referenceModel.IsCollection)
                targetType = EntryConvert.ElementType(targetType);
            var typeConstraints = MergeTypeConstraints(property, targetType, overrides);
            referenceModel.SupportedTypes = TypeController.SupportedTypes(typeConstraints).Select(t => t.Name).ToArray();

            return referenceModel;
        }

        #endregion
    }
}
