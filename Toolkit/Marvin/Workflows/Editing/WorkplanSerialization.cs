using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Implementation of <see cref="ICustomSerialization"/> for workplanstep properties
    /// </summary>
    internal class WorkplanSerialization : DefaultSerialization
    {
        private readonly IWorkplanSource _source;

        /// <summary>
        /// Static instance that can only be used for non workplan properties
        /// </summary>
        public static ICustomSerialization Simple = new WorkplanSerialization();

        /// <summary>
        /// Private constructor for the static instance only
        /// </summary>
        private WorkplanSerialization()
        {
        }

        /// <summary>
        /// Workplan source used to read properties of type <see cref="IWorkplan"/>
        /// </summary>
        public WorkplanSerialization(IWorkplanSource source)
        {
            _source = source;
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override IEnumerable<PropertyInfo> ReadFilter(Type sourceType)
        {
            // Only properties with read filter
            foreach (var property in base.ReadFilter(sourceType))
            {
                // Filter properties without initializer
                if(property.GetCustomAttribute(typeof(InitializerAttribute)) == null)
                    continue;

                // Filter properties of type workplan
                if(IsWorkplanReference(property))
                    continue;

                yield return property;
            }
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
        {
            // Break recursion for workplans
            return IsWorkplanReference(sourceType) ? new MappedProperty[0] : base.WriteFilter(sourceType, encoded);
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override object PropertyValue(PropertyInfo property, Entry mappedEntry, object currentValue)
        {
            // Override mechanism to load workplan references
            if (IsWorkplanReference(property))
            {
                if (_source == null)
                    return currentValue;

                var newId = long.Parse(mappedEntry.Value.Current);
                var currentWorkplan = (IWorkplan)currentValue;
                return currentWorkplan.Id == newId ? currentWorkplan : _source.Load(newId);
            }
            return base.PropertyValue(property, mappedEntry, currentValue);
        }

        /// <summary>
        /// Build collection from entries
        /// </summary>
        public static object BuildCollection(Type collectionType, Entry rootEntry)
        {
            // TODO: Not the most elegant solution
            var elemType = EntryConvert.ElementType(collectionType);
            var collection = (IList)CollectionBuilder(collectionType, null, rootEntry);
            for (int i = 0; i < rootEntry.SubEntries.Count; i++)
            {
                collection[i] = EntryConvert.CreateInstance(elemType, rootEntry.SubEntries[i]);
            }
            return collection;
        }

        /// <summary>
        /// Check if a property is a reference to another instance of <see cref="IWorkplan"/>
        /// </summary>
        public static bool IsWorkplanReference(PropertyInfo property)
        {
            return IsWorkplanReference(property.PropertyType);
        }

        /// <summary>
        /// Check if a type is a reference to a workplan
        /// </summary>
        public static bool IsWorkplanReference(Type type)
        {
            return typeof(IWorkplan).IsAssignableFrom(type);
        }
    }
}