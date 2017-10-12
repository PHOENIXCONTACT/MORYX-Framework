using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Non-generated part of the serialization helper
    /// </summary>
    public static partial class EntryConvert
    {
        /// <summary>
        /// Default strategy instance
        /// </summary>
        internal static ICustomSerialization Serialization = new DefaultSerialization();

        /// <summary>
        /// Check if a certain type is what we consider a collection
        /// </summary>
        public static bool IsCollection(Type propertyType)
        {
            return typeof(IEnumerable).IsAssignableFrom(propertyType) && typeof(string) != propertyType;
        }

        /// <summary>
        /// Check if the type is either a value type or a string
        /// </summary>
        public static bool ValueOrStringType(Type propertyType)
        {
            return propertyType.IsValueType || propertyType == typeof(string);
        }

        /// <summary>
        /// Helper method to determine the element type of arrays or lists
        /// </summary>
        public static Type ElementType(Type collectionType)
        {
            // If it is a dictionary create a key value pair
            if (collectionType.GetGenericArguments().Length == 2) // TODO: Better criteria for dictionary
                return collectionType.GenericTypeArguments[1];

            // otherwise create the element's type. List<Test> --> Test
            return collectionType.IsArray ? collectionType.GetElementType() : collectionType.GenericTypeArguments[0];
        }

        /// <summary>
        /// Name of entry for list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Obsolete("In the future we will not provide access to the method")]
        public static string GetEntryName(object item)
        {
            return CollectionStrategyTools.GetEntryName(item);
        }

        #region Encode

        /// <summary>
        /// Convert a single property into a simple entry
        /// </summary>
        /// <returns>Covnerted property</returns>
        public static Entry EncodeProperty(PropertyInfo property)
        {
            return EncodeProperty<Entry>(property, Serialization);
        }

        /// <summary>
        /// Convert a single property into a simple entry using a custom strategy
        /// </summary>
        /// <returns>Covnerted property</returns>
        public static Entry EncodeProperty(PropertyInfo property, ICustomSerialization customSerialization)
        {
            return EncodeProperty<Entry>(property, customSerialization);
        }

        /// <summary>
        /// Convert a single property into a derived type of entry
        /// </summary>
        /// <typeparam name="T">Type of property representation</typeparam>
        /// <returns>Covnerted property</returns>
        public static T EncodeProperty<T>(PropertyInfo property)
            where T : Entry, new()
        {
            return EncodeProperty<T>(property, Serialization);
        }

        /// <summary>
        /// Convert a single property into a derived type of entry using a custom strategy
        /// </summary>
        /// <typeparam name="T">Type of property representation</typeparam>
        /// <returns>Covnerted property</returns>
        public static T EncodeProperty<T>(PropertyInfo property, ICustomSerialization customSerialization)
            where T : Entry, new()
        {
            // Fill with default if entry is null
            var descriptionAtt = property.GetCustomAttribute<DescriptionAttribute>();
            var displayNameAtt = property.GetCustomAttribute<DisplayNameAttribute>();
            var entry = new T
            {
                Key = new EntryKey
                {
                    Identifier = property.Name,
                    Name = displayNameAtt == null ? property.Name : displayNameAtt.DisplayName
                },
                Description = descriptionAtt?.Description,
                Value = CreateEntryValue(property, customSerialization),
                Validation = customSerialization.CreateValidation(property)
            };

            // Include prototypes for collections and classes
            var prototypes = Prototypes(property, entry, customSerialization);
            entry.Prototypes.AddRange(prototypes);

            return entry;
        }

        /// <see cref="ICustomSerialization"/>
        private static EntryValue CreateEntryValue(PropertyInfo property, ICustomSerialization customSerialization)
        {
            // Prepare object
            var entryValue = new EntryValue
            {
                Type = TransformType(property.PropertyType),
                IsReadOnly = !property.CanWrite,
                Possible = customSerialization.PossibleValues(property),
            };

            // Get most basic default
            var defaultAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttribute != null)
                entryValue.Default = defaultAttribute.Value.ToString();
            else if (entryValue.Possible != null && entryValue.Possible.Length >= 1)
                entryValue.Default = entryValue.Possible[0];
            else if (property.PropertyType.IsValueType)
                entryValue.Default = Activator.CreateInstance(property.PropertyType).ToString();

            // Value types should have the default value as current value
            if (property.PropertyType.IsValueType)
                entryValue.Current = entryValue.Default;

            return entryValue;
        }

        /// <summary>
        /// Create prototypes for possible values of an entry
        /// </summary>
        private static IEnumerable<Entry> Prototypes(PropertyInfo property, Entry parent, ICustomSerialization customSerialization)
        {
            if (parent.Value.Type != EntryValueType.Collection && parent.Value.Type != EntryValueType.Class)
                yield break;

            var possibleElementValues = IsCollection(property.PropertyType) 
                ? customSerialization.PossibleElementValues(property) : null;
            foreach (var prototype in customSerialization.Prototypes(property))
            {
                var prototypeEntry = Prototype(prototype, customSerialization);
                prototypeEntry.Value.Possible = possibleElementValues;
                yield return prototypeEntry;
            }
        }

        /// <summary>
        /// Create a prototype entry for a certain type
        /// </summary>
        public static Entry Prototype(EntryPrototype prototype)
        {
            return Prototype(prototype, Serialization);
        }

        /// <summary>
        /// Create a prototype entry for a certain type
        /// </summary>
        public static Entry Prototype(EntryPrototype type, ICustomSerialization customSerialization)
        {
            var encoded = new Entry
            {
                Key = new EntryKey
                {
                    Name = type.Key,
                    Identifier = EntryKey.ProtoIdentifier
                }
            };

            var prototypeType = type.Prototype.GetType();
            if (ValueOrStringType(prototypeType))
            {
                var value = type.Prototype.ToString();
                encoded.Value = new EntryValue
                {
                    Current = value,
                    Default = value,
                    Type = TransformType(type.Prototype.GetType())
                };
            }
            else
            {
                encoded.Value = new EntryValue
                {
                    Current = type.Key,
                    Type = EntryValueType.Class
                };
            }

            if (encoded.Value.Type != EntryValueType.Class)
                return encoded;

            var properties = EncodeObject(type.Prototype, customSerialization);
            encoded.SubEntries.AddRange(properties);
            return encoded;
        }

        /// <summary>
        /// Convert class into a list of entries using default strategy
        /// </summary>
        public static IEnumerable<Entry> EncodeClass(Type objType)
        {
            return EncodeClass<Entry>(objType, Serialization);
        }

        /// <summary>
        /// Convert class into a list of generic entries using custom strategy
        /// </summary>
        public static IEnumerable<Entry> EncodeClass(Type objType, ICustomSerialization customSerialization)
        {
            return EncodeClass<Entry>(objType, customSerialization);
        }

        /// <summary>
        /// Convert class into a list of generic entries using default strategy
        /// </summary>
        public static IEnumerable<T> EncodeClass<T>(Type objType)
            where T : Entry, new()
        {
            return EncodeClass<T>(objType, Serialization);
        }

        /// <summary>
        /// Convert class into a list of generic entries using custom strategy
        /// </summary>
        public static IEnumerable<T> EncodeClass<T>(Type objType, ICustomSerialization customSerialization)
            where T : Entry, new()
        {
            var filtered = customSerialization.ReadFilter(objType);
            foreach (var property in filtered)
            {
                var converted = EncodeProperty<T>(property, customSerialization);
                // Assign default to current
                var value = converted.Value;
                value.Current = value.Default;
                // Recursive call for classes
                if (converted.Value.Type == EntryValueType.Class)
                {
                    // Convert sub entries for classes
                    var subentries = EncodeClass(property.PropertyType, customSerialization);
                    converted.SubEntries.AddRange(subentries);
                }
                yield return converted;
            }
        }

        /// <summary>
        /// Convert object instance using default strategy
        /// </summary>
        public static IEnumerable<Entry> EncodeObject(object instance)
        {
            return EncodeObject<Entry>(instance, Serialization);
        }

        /// <summary>
        /// Convert object instance using custom strategy
        /// </summary>
        public static IEnumerable<Entry> EncodeObject(object instance, ICustomSerialization customSerialization)
        {
            return EncodeObject<Entry>(instance, customSerialization);
        }

        /// <summary>
        /// Convert object instance using default strategy
        /// </summary>
        public static IEnumerable<T> EncodeObject<T>(object instance)
            where T : Entry, new()
        {
            return EncodeObject<T>(instance, Serialization);
        }

        /// <summary>
        /// Convert object instance using custom strategy
        /// </summary>
        public static IEnumerable<T> EncodeObject<T>(object instance, ICustomSerialization customSerialization)
            where T : Entry, new()
        {
            var filtered = customSerialization.ReadFilter(instance.GetType());
            foreach (var property in filtered)
            {
                var value = property.GetValue(instance);

                var converted = EncodeProperty<T>(property, customSerialization);
                switch (converted.Value.Type)
                {
                    case EntryValueType.Collection:
                        // Get collection and iterate if it has entries
                        var enumurable = value as IEnumerable;
                        if (enumurable == null)
                            break;

                        var possibleElementValues = customSerialization.PossibleElementValues(property);
                        var strategy = CreateStrategy(value, value, property.PropertyType, customSerialization);
                        var subentries = strategy.Serialize();
                        foreach (var entry in subentries)
                        {
                            entry.Value.Possible = possibleElementValues;
                            converted.SubEntries.Add(entry);
                        }
                        break;

                    case EntryValueType.Class:
                        Type type;
                        if (value == null)
                        {
                            type = property.PropertyType;
                            subentries = EncodeClass(type, customSerialization);
                        }
                        else
                        {
                            type = value.GetType();
                            subentries = EncodeObject(value, customSerialization);
                        }
                        converted.Value.Current = type.Name;
                        converted.SubEntries.AddRange(subentries);
                        break;

                    default:
                        converted.Value.Current = value?.ToString();
                        break;

                }
                yield return converted;
            }
        }

        #endregion

        #region Decode

        /// <summary>
        /// Create typed instance from encoded properties
        /// </summary>
        public static T CreateInstance<T>(IEnumerable<Entry> encoded)
            where T : class, new()
        {
            var instance = new T();
            UpdateInstance(instance, encoded, Serialization);
            return instance;
        }

        /// <summary>
        /// Create typed instance from encoded properties
        /// </summary>
        public static T CreateInstance<T>(IEnumerable<Entry> encoded, ICustomSerialization customSerialization)
            where T : class, new()
        {
            var instance = new T();
            UpdateInstance(instance, encoded, customSerialization);
            return instance;
        }

        /// <summary>
        /// Create typed instance from config model using entry for specialized types
        /// </summary>
        public static T CreateInstance<T>(Entry encoded)
            where T : class
        {
            return (T)CreateInstance(typeof(T), encoded, Serialization);
        }

        /// <summary>
        /// Create typed instance from config model using entry for specialized types
        /// </summary>
        public static T CreateInstance<T>(Entry encoded, ICustomSerialization customSerialization)
            where T : class
        {
            return (T)CreateInstance(typeof(T), encoded, customSerialization);
        }

        /// <summary>
        /// Create object instance from config model using entry for specialized types
        /// </summary>
        public static object CreateInstance(Type type, Entry encoded)
        {
            return CreateInstance(type, encoded, Serialization);
        }

        /// <summary>
        /// Create object instance from config model using entry for specialized types
        /// </summary>
        public static object CreateInstance(Type type, Entry encoded, ICustomSerialization customSerialization)
        {
            var instance = customSerialization.CreateInstance(type, encoded);
            return UpdateInstance(instance, encoded.SubEntries, customSerialization);
        }

        /// <summary>
        /// Create object instance from encoded properties
        /// </summary>
        public static object CreateInstance(Type type, IEnumerable<Entry> encoded)
        {
            return CreateInstance(type, encoded, Serialization);
        }

        /// <summary>
        /// Create object instance from encoded properties
        /// </summary>
        public static object CreateInstance(Type type, IEnumerable<Entry> encoded, ICustomSerialization customSerialization)
        {
            var instance = Activator.CreateInstance(type);
            return UpdateInstance(instance, encoded, customSerialization);
        }

        /// <summary>
        /// Update existing object from encoded values
        /// </summary>
        public static object UpdateInstance(object instance, IEnumerable<Entry> encoded)
        {
            return UpdateInstance(instance, encoded, Serialization);
        }

        /// <summary>
        /// Update existing object from encoded values
        /// </summary>
        public static object UpdateInstance(object instance, IEnumerable<Entry> encoded, ICustomSerialization customSerialization)
        {
            var filtered = customSerialization.WriteFilter(instance.GetType(), encoded);
            foreach (var mapped in filtered)
            {
                var property = mapped.Property;
                var propertyType = mapped.Property.PropertyType;

                // Try to assign value to the property
                var currentValue = mapped.Property.GetValue(instance);
                var value = mapped.Entry == null
                    ? customSerialization.DefaultValue(mapped.Property, currentValue)
                    : customSerialization.PropertyValue(mapped, currentValue);
                
                // Value types and strings do not need recursion
                if (ValueOrStringType(propertyType))
                {
                    
                }
                // Update collection from entry
                else if (value is ICollection && mapped.Entry != null)
                {
                    // Pick collection strategy
                    var strategy = CreateStrategy(value, currentValue, propertyType, customSerialization);
                    UpdateCollection(currentValue, mapped, strategy, customSerialization);
                }
                // Update class
                else if (propertyType.IsClass)
                {
                    if (mapped.Entry == null)
                        UpdateInstance(value, new Entry[0], customSerialization);
                    else
                        UpdateInstance(value, mapped.Entry.SubEntries, customSerialization);
                }
                else
                {
                    //TODO: what to do with types which cannot be converted???
                    //Sample: HashSet<T> is not implementing ICollection (but ICollection<T>)
                    //Extend UnitTests when fixed.
                }

                // Write new value to property
                if (property.CanWrite && value != currentValue)
                {
                    mapped.Property.SetValue(instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Update or fill the collection using the mapping entry
        /// </summary>
        private static void UpdateCollection(object currentValue, MappedProperty mapped, ICollectionStrategy strategy, ICustomSerialization customSerialization)
        {
            var rootEntry = mapped.Entry;
            var currentCollection = currentValue as ICollection;

            // Loop over the collection and update the entries that are still present
            if (currentCollection != null)
            {
                foreach (var key in strategy.Keys())
                {
                    var item = strategy.ElementAt(key);
                    var match = rootEntry.SubEntries.Find(se => se.Key.Identifier == key);
                    if (match == null)
                    {
                        strategy.Removed(key);
                        continue;
                    }

                    if (match.Value.Type < EntryValueType.Class)
                    {
                        item = CreatePrimitiveOrEnum(mapped.Property.PropertyType, match.Value);
                    }
                    else
                    {
                        UpdateInstance(item, match.SubEntries, customSerialization);
                    }
                    strategy.Updated(match.Key, item);
                }
            }

            // Add new entries to the collection
            foreach (var subEntry in rootEntry.SubEntries.Where(se => se.Key.Identifier == EntryKey.CreatedIdentifier))
            {
                object item;
                // All value types
                if (subEntry.Value.Type < EntryValueType.Class)
                {
                    // Create value type
                    item = CreatePrimitiveOrEnum(mapped.Property.PropertyType, subEntry.Value);
                }
                else
                {
                    // Create and update reference types
                    item = customSerialization.CreateInstance(mapped, subEntry);
                    item = UpdateInstance(item, subEntry.SubEntries, customSerialization);
                }
                strategy.Added(subEntry.Key, item);
            }

            // Finalize all operations
            strategy.Flush();
        }

        /// <summary>
        /// Create object for primitive values like int/string or enums
        /// </summary>
        private static object CreatePrimitiveOrEnum(Type propertyType, EntryValue entryValue)
        {
            object item;
            // All value types
            if (entryValue.Type < EntryValueType.Enum)
            {
                // Create value type
                item = ToObject(entryValue.Type, entryValue.Current);
            }
            else
            {
                var elementType = ElementType(propertyType);
                item = ToObject(elementType, entryValue.Current);
            }
            return item;
        }

        /// <summary>
        /// Create strategy for collection
        /// </summary>
        private static ICollectionStrategy CreateStrategy(object collection, object currentCollection, Type collectionType, ICustomSerialization serialization)
        {
            ICollectionStrategy strategy;
            if (collectionType.IsArray)
            {
                strategy = new ArrayStrategy((Array)collection, currentCollection as Array, serialization);
            }
            else if (collection is IDictionary)
            {
                strategy = new DictionaryStrategy((IDictionary)collection, serialization);
            }
            else if (collection is IList)
            {
                strategy = new ListStrategy((IList)collection, serialization);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported collection type {collectionType}");
            }

            return strategy;
        }

        #endregion
    }
}