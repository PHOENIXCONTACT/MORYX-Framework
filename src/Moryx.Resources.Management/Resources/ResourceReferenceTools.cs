// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Collection of reflection methods necessary for resource linking
    /// </summary>
    internal static class ResourceReferenceTools
    {
        /// <summary>
        /// Initialize all <see cref="IReferences{TResource}"/> collections of a <see cref="Resource"/>
        /// </summary>
        /// <param name="instance"></param>
        public static void InitializeCollections(Resource instance)
        {
            var resourceType = instance.GetType();
            // Iterate all references and provide reference collections
            var overrides = new Dictionary<PropertyInfo, ReferenceOverrideAttribute>();
            foreach (var property in CollectionReferenceProperties(resourceType))
            {
                var attribute = property.GetCustomAttribute<ReferenceOverrideAttribute>();
                if (attribute == null)
                    // Create collection and set on property
                    CreateCollection(instance, property);
                else
                    // Save overrides for later
                    overrides[property] = attribute;
            }

            // Now set the reference overrides
            foreach (var pair in overrides)
            {
                // Fetch already created reference collection
                var targetName = pair.Value.Source;
                var target = resourceType.GetProperty(targetName);
                var sourceCollection = (IReferenceCollection)target.GetValue(instance);

                // Create new reference collection that shares the UnderlyingCollection
                var property = pair.Key;
                CreateCollection(instance, property, sourceCollection.UnderlyingCollection, target);
            }
        }

        /// <summary>
        /// Create a <see cref="ReferenceCollection{TResource}"/> instance
        /// </summary>
        /// <param name="instance">The resource instance to create the collection for</param>
        /// <param name="property">The collection property that should be filled by this collection</param>
        /// <param name="underlyingCollection">The base collection wrapped in the reference collection. This can be null for non-override properties</param>
        /// <param name="targetProperty">Target property of the collection. For non-overrides this equals <paramref name="property"/>.</param>
        private static void CreateCollection(Resource instance, PropertyInfo property, ICollection<IResource> underlyingCollection = null, PropertyInfo targetProperty = null)
        {
            // Set target property to property if it is not given
            if (targetProperty == null)
                targetProperty = property;

            // Create underlying collection if it is not given
            if (underlyingCollection == null)
                underlyingCollection = new List<IResource>();

            var propertyType = property.PropertyType;
            var referenceType = propertyType.GetGenericArguments()[0]; // Type of resource from ICollection<ResourceType>
            var collectionType = typeof(ReferenceCollection<>).MakeGenericType(referenceType); // Make generic ReferenceCollection

            // Create collection and set on instance property
            var value = Activator.CreateInstance(collectionType, instance, targetProperty, underlyingCollection);
            property.SetValue(instance, value);
        }

        /// <summary>
        /// Get all collections of a type that are configured for AutoSave
        /// </summary>
        public static ICollection<IReferenceCollection> GetAutoSaveCollections(Resource instance)
        {
            return (from collectionProperty in CollectionReferenceProperties(instance.GetType())
                    let refAtt = collectionProperty.GetCustomAttribute<ResourceReferenceAttribute>()
                    let overrideAtt = collectionProperty.GetCustomAttribute<ReferenceOverrideAttribute>()
                    where (refAtt?.AutoSave ?? false) || (overrideAtt?.AutoSave ?? false)
                    select (IReferenceCollection)collectionProperty.GetValue(instance)).ToList();
        }

        /// <summary>
        /// Find all reference properties on a resource type
        /// </summary>
        public static IEnumerable<PropertyInfo> CollectionReferenceProperties(Type resourceType)
        {
            return from referenceProperty in ReferenceProperties(resourceType, true)
                   let propertyType = referenceProperty.PropertyType
                   where propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IReferences<>)
                   select referenceProperty;
        }

        /// <summary>
        /// All properties of a resource type that represent references or reference overrides
        /// </summary>
        public static IEnumerable<PropertyInfo> ReferenceProperties(Type resourceType, bool includeOverrides)
        {
            return from property in resourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where property.CanWrite &&
                         (Attribute.IsDefined(property, typeof(ResourceReferenceAttribute))
                          || includeOverrides && Attribute.IsDefined(property, typeof(ReferenceOverrideAttribute)))
                   select property;
        }
    }
}
