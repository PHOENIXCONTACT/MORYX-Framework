using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Tools;

namespace Moryx.Serialization
{
    /// <summary>
    /// Non-generated part of the customSerialization helper
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
        /// Check if the property type represents a dictionary
        /// </summary>
        public static bool IsDictionary(Type collectionType)
        {
            if (typeof(IDictionary).IsAssignableFrom(collectionType))
                return true;

            return collectionType.IsGenericType
                   && collectionType.GetGenericTypeDefinition() == typeof(IDictionary<,>);
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
            if (IsDictionary(collectionType))
                return collectionType.GenericTypeArguments[1];

            // otherwise create the element's type. List<Test> --> Test
            return collectionType.IsArray ? collectionType.GetElementType() : collectionType.GenericTypeArguments[0];
        }

        #region Encode

        /// <summary>
        /// Convert a single property into a simple entry
        /// </summary>
        /// <returns>Covnerted property</returns>
        public static Entry EncodeProperty(PropertyInfo property)
        {
            return EncodeProperty(property, Serialization);
        }

        /// <summary>
        /// Convert a single property into a derived type of entry using a custom strategy
        /// </summary>
        /// <returns>Converted property</returns>
        public static Entry EncodeProperty(PropertyInfo property, ICustomSerialization customSerialization)
        {
            // Fill with default if entry is null
            var entry = new Entry
            {
                DisplayName = property.GetDisplayName() ?? property.Name,
                Identifier = property.Name,
                Description = property.GetDescription(),
                Value = CreateEntryValue(property, customSerialization),
                Validation = customSerialization.CreateValidation(property.PropertyType, property)
            };

            // No prototypes for readonly entries or entries without flexible sub-entries
            if (entry.Value.IsReadOnly || entry.Value.Type != EntryValueType.Collection && entry.Value.Type != EntryValueType.Class)
                return entry;

            // Determine and convert prototypes
            var prototypes = Prototypes(property.PropertyType, property, customSerialization);
            entry.Prototypes.AddRange(prototypes);

            return entry;
        }

        /// <see cref="ICustomSerialization"/>
        private static EntryValue CreateEntryValue(PropertyInfo property, ICustomSerialization customSerialization)
        {
            // Set if the current entry is readonly by checking if the property has a setter
            // or the ReadOnlyAttribute was set to true
            var isReadOnly = !property.CanWrite;
            if (!isReadOnly)
            {
                var readOnlyAtt = property.GetCustomAttribute<ReadOnlyAttribute>();
                isReadOnly = readOnlyAtt?.IsReadOnly ?? false;
            }

            // Prepare object
            var entryValue = new EntryValue
            {
                Type = TransformType(property.PropertyType),
                UnitType = customSerialization.GetUnitTypeByAttributes(property),
                IsReadOnly = isReadOnly,
                Possible = customSerialization.PossibleValues(property.PropertyType, property)
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
            if (ValueOrStringType(property.PropertyType))
                entryValue.Current = ConvertToString(entryValue.Default, customSerialization.FormatProvider);

            return entryValue;
        }

        /// <summary>
        /// Create prototypes for possible values of an entry
        /// </summary>
        private static IEnumerable<Entry> Prototypes(Type memberType, ICustomAttributeProvider customAttributeProvider, ICustomSerialization customSerialization)
        {
            var possibleElementValues = IsCollection(memberType)
                ? customSerialization.PossibleElementValues(memberType, customAttributeProvider)
                : customSerialization.PossibleValues(memberType, customAttributeProvider);
            var validation = customSerialization.CreateValidation(memberType, customAttributeProvider);

            foreach (var prototype in customSerialization.Prototypes(memberType, customAttributeProvider))
            {
                var prototypeEntry = Prototype(prototype, customSerialization);
                prototypeEntry.Validation = validation;
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
            Entry encoded;
            var transformedType = TransformType(type.Prototype.GetType());
            if (transformedType != EntryValueType.Class)
            {
                var value = type.Prototype.ToString();
                var valueType = type.Prototype.GetType();
                encoded = new Entry
                {
                    DisplayName = valueType.Name,
                    Identifier = Entry.CreatedIdentifier,
                    Value = new EntryValue
                    {
                        Current = value,
                        Default = value,
                        Type = TransformType(valueType)
                    }
                };
            }
            else
            {
                encoded = EncodeObject(type.Prototype, customSerialization);
            }

            return encoded;
        }

        /// <summary>
        /// Convert class into a list of entries using default strategy
        /// </summary>
        public static Entry EncodeClass(Type objType)
        {
            return EncodeClass(objType, Serialization);
        }
        /// <summary>
        /// Convert class into a list of generic entries using custom strategy
        /// </summary>
        public static Entry EncodeClass(Type objType, ICustomSerialization customSerialization)
        {
            var encodedClass = CreateFromType(objType, customSerialization);

            var filtered = customSerialization.GetProperties(objType);
            foreach (var property in filtered)
            {
                var converted = EncodeProperty(property, customSerialization);
                // Assign default to current
                var value = converted.Value;
                value.Current = ConvertToString(value.Default, customSerialization.FormatProvider);
                // Recursive call for classes
                if (converted.Value.Type == EntryValueType.Class)
                {
                    // Convert sub entries for classes
                    var subentry = EncodeClass(property.PropertyType, customSerialization);
                    converted.SubEntries.AddRange(subentry.SubEntries);
                }

                encodedClass.SubEntries.Add(converted);
            }

            return encodedClass;
        }

        /// <summary>
        /// Convert object instance using default strategy
        /// </summary>
        public static Entry EncodeObject(object instance)
        {
            return EncodeObject(instance, Serialization);
        }

        /// <summary>
        /// Convert object instance using custom strategy
        /// </summary>
        public static Entry EncodeObject(object instance, ICustomSerialization customSerialization)
        {
            var instanceType = instance.GetType();
            var isValueType = ValueOrStringType(instanceType);

            var converted = CreateFromType(instanceType, customSerialization);

            // Value or string are easily value encoded
            if (isValueType)
            {
                converted.Value.Current = ConvertToString(instance, customSerialization.FormatProvider);
                return converted;
            }

            // Collections are encoded as a root entry with subentries
            if (converted.Value.Type == EntryValueType.Collection)
            {
                var possibleElementValues = customSerialization.PossibleElementValues(instanceType, instanceType);
                var strategy = CreateStrategy(instance, instance, instanceType, customSerialization);
                foreach (var entry in strategy.Serialize())
                {
                    entry.Value.Possible = possibleElementValues;
                    converted.SubEntries.Add(entry);
                }
                return converted;
            }

            // Classes are encoded per-property recursively
            var filtered = customSerialization.GetProperties(instance.GetType());
            foreach (var property in filtered)
            {
                var convertedProperty = EncodeProperty(property, customSerialization);

                object value;
                try
                {
                    value = property.GetValue(instance);
                }
                catch (Exception ex)
                {
                    value = ex;
                    // Change type in case of exception
                    convertedProperty.Value.Type = EntryValueType.Exception;
                }

                switch (convertedProperty.Value.Type)
                {
                    case EntryValueType.Collection:
                        // Get collection and iterate if it has entries
                        if (value == null)
                            break;

                        var possibleElementValues = customSerialization.PossibleElementValues(property.PropertyType, property);
                        var strategy = CreateStrategy(value, value, property.PropertyType, customSerialization);
                        foreach (var entry in strategy.Serialize())
                        {
                            entry.Value.Possible = possibleElementValues;
                            convertedProperty.SubEntries.Add(entry);
                        }
                        break;
                    case EntryValueType.Class:
                        var subEntry = value == null
                            ? EncodeClass(property.PropertyType, customSerialization)
                            : EncodeObject(value, customSerialization);
                        convertedProperty.Value.Current = ConvertToString(subEntry.Value.Current, customSerialization.FormatProvider);
                        convertedProperty.SubEntries = subEntry.SubEntries;
                        break;
                    case EntryValueType.Exception:
                        convertedProperty.Value.Current = ExceptionPrinter.Print((Exception)value);
                        break;
                    case EntryValueType.Stream:
                        var stream = value as Stream;
                        if (stream != null)
                        {
                            if (stream.CanSeek)
                                stream.Seek(0, SeekOrigin.Begin);

                            convertedProperty.Value.Current = ConvertToBase64(stream);
                        }
                        break;
                    default:
                        convertedProperty.Value.Current = ConvertToString(value ?? convertedProperty.Value.Default, customSerialization.FormatProvider);
                        break;
                }

                converted.SubEntries.Add(convertedProperty);
            }

            return converted;
        }

        /// <summary>
        /// Create basic <see cref="Entry"/> instance for a given object type
        /// </summary>
        private static Entry CreateFromType(Type objectType, ICustomSerialization serialization)
        {
            var entry = new Entry
            {
                DisplayName = objectType.GetDisplayName() ?? objectType.Name,
                Identifier = objectType.Name,
                Value = new EntryValue
                {
                    Current = objectType.Name,
                    Default = objectType.Name,
                    Type = TransformType(objectType)
                },
                Validation = serialization.CreateValidation(objectType, objectType),
                Description = objectType.GetDescription()
            };
            return entry;
        }

        /// <summary>
        /// Read a stream as Base64
        /// </summary>
        private static string ConvertToBase64(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Encode a <see cref="MethodBase"/> to the transmittable <see cref="MethodEntry"/>
        /// using <see cref="DefaultSerialization"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodEntry EncodeMethod(MethodBase method)
        {
            return EncodeMethod(method, Serialization);
        }

        /// <summary>
        /// Encode a <see cref="MethodBase"/> to the transmittable <see cref="MethodEntry"/>
        /// </summary>
        public static MethodEntry EncodeMethod(MethodBase method, ICustomSerialization serialization)
        {
            return new MethodEntry
            {
                Name = method.Name,
                IsConstructor = method.IsConstructor,
                DisplayName = method.GetDisplayName() ?? method.Name,
                Description = method.GetDescription(),
                Parameters = new Entry
                {
                    DisplayName = "Root",
                    Identifier = "Root",
                    Value = new EntryValue { Type = EntryValueType.Class },
                    SubEntries = method.GetParameters().Select(p => ConvertParameter(p, serialization)).ToList()
                }
            };
        }

        /// <summary>
        /// Encode all methods of an object using <see cref="DefaultSerialization"/>
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeMethods(object source)
        {
            return EncodeMethods(source.GetType(), Serialization);
        }

        /// <summary>
        /// Encode all methods of an object using a custom customSerialization
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeMethods(object source, ICustomSerialization serialization)
        {
            return EncodeMethods(source.GetType(), serialization);
        }

        /// <summary>
        /// Encode all methods of class using <see cref="DefaultSerialization"/>
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeMethods(Type objType)
        {
            return EncodeMethods(objType, Serialization);
        }

        /// <summary>
        /// Encode all methods of a type using a custom customSerialization
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeMethods(Type objType, ICustomSerialization serialization)
        {
            var methods = serialization.GetMethods(objType);
            return methods.Select(m => EncodeMethod(m, serialization));
        }

        /// <summary>
        /// Encode all constructors of a class
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeConstructors(Type objType)
        {
            return EncodeConstructors(objType, Serialization);
        }

        /// <summary>
        /// Encode all constructors of a class
        /// </summary>
        public static IEnumerable<MethodEntry> EncodeConstructors(Type objType, ICustomSerialization serialization)
        {
            var constructors = objType.GetConstructors();
            return constructors.Select(c => EncodeMethod(c, serialization));
        }

        /// <summary>
        /// Convert a method parameter to our standard <see cref="Entry"/> format
        /// </summary>
        private static Entry ConvertParameter(ParameterInfo parameter, ICustomSerialization serialization)
        {
            var parameterType = parameter.ParameterType;
            var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue.ToString() : null;

            var parameterModel = new Entry
            {
                DisplayName = parameter.GetDisplayName() ?? parameter.Name,
                Identifier = parameter.Name,
                Description = parameter.GetDescription(),
                Value = new EntryValue
                {
                    Type = TransformType(parameter.ParameterType),
                    UnitType = serialization.GetUnitTypeByAttributes(parameter),
                    Current = defaultValue,
                    Default = defaultValue,
                    Possible = serialization.PossibleValues(parameterType, parameter)
                },
                Validation = serialization.CreateValidation(parameterType, parameter)
            };

            switch (parameterModel.Value.Type)
            {
                case EntryValueType.Class:
                    parameterModel.Value.Current = parameterType.Name;
                    parameterModel.Prototypes.AddRange(Prototypes(parameterType, parameter, serialization));
                    parameterModel.SubEntries = EncodeClass(parameterType, serialization).SubEntries;
                    break;
                case EntryValueType.Collection:
                    var elemType = ElementType(parameterType);
                    parameterModel.Value.Current = elemType.Name;
                    parameterModel.Prototypes.AddRange(Prototypes(parameterType, parameter, serialization));
                    break;
            }

            return parameterModel;
        }

        #endregion

        #region Decode

        /// <summary>
        /// Create typed instance from encoded properties
        /// </summary>
        public static T CreateInstance<T>(Entry encoded)
            where T : class, new()
        {
            var instance = new T();
            UpdateInstance(instance, encoded, Serialization);
            return instance;
        }

        /// <summary>
        /// Create typed instance from encoded properties
        /// </summary>
        public static T CreateInstance<T>(Entry encoded, ICustomSerialization customSerialization)
            where T : class, new()
        {
            var instance = new T();
            UpdateInstance(instance, encoded, customSerialization);
            return instance;
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
            return UpdateInstance(instance, encoded, customSerialization);
        }

        /// <summary>
        /// Create object instance from config model using entry for specialized types
        /// </summary>
        public static object CreateInstance(Type type, MethodEntry encodedConstructor)
        {
            return CreateInstance(type, encodedConstructor, Serialization);
        }

        /// <summary>
        /// Create object instance from config model using entry for specialized types
        /// </summary>
        public static object CreateInstance(Type type, MethodEntry encodedConstructor, ICustomSerialization customSerialization)
        {
            // Retrieve the most specific constructor matched by the encoded method entry
            var constructor = (from ctor in type.GetConstructors()
                               let parameters = ctor.GetParameters()
                               where parameters.Length == encodedConstructor.Parameters.SubEntries.Count
                                     && ParametersProvided(parameters, encodedConstructor)
                               select ctor).First();
            var arguments = ConvertArguments(constructor, encodedConstructor, customSerialization);
            var instance = constructor.Invoke(arguments);
            return instance;
        }

        /// <summary>
        /// Update existing object from encoded values
        /// </summary>
        public static object UpdateInstance(object instance, Entry encoded)
        {
            return UpdateInstance(instance, encoded, Serialization);
        }

        /// <summary>
        /// Update existing object from encoded values
        /// </summary>
        public static object UpdateInstance(object instance, Entry encoded, ICustomSerialization customSerialization)
        {
            var filtered = customSerialization.WriteFilter(instance.GetType(), encoded.SubEntries);
            foreach (var mapped in filtered)
            {
                var property = mapped.Property;
                var propertyType = mapped.Property.PropertyType;
                var doNotUpdateProperty = false; 

                // Do not operate on faulty properties or read-only properties
                // For security reasons read the flag from the property again
                if (mapped.Entry?.Value.Type == EntryValueType.Exception || property.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly == true)
                    continue;

                // Try to assign value to the property
                var currentValue = mapped.Property.GetValue(instance);
                var value = mapped.Entry == null
                    ? customSerialization.DefaultValue(property, currentValue)
                    : customSerialization.ConvertValue(propertyType, property, mapped.Entry, currentValue);

                // Value types, strings and streams do not need recursion
                if (ValueOrStringType(propertyType) || typeof(Stream).IsAssignableFrom(propertyType))
                {

                }
                // Update collection from entry
                else if (IsCollection(propertyType) && mapped.Entry != null)
                {
                    // Pick collection strategy
                    var strategy = CreateStrategy(value, currentValue, propertyType, customSerialization, mapped.Property, instance);
                    UpdateCollection(currentValue, mapped.Property.PropertyType, mapped.Property, mapped.Entry, strategy, customSerialization);
                    if (strategy is ArrayIListStrategy)
                        doNotUpdateProperty = true;
                }
                // Update class
                else if (propertyType.IsClass)
                {
                    UpdateInstance(value, mapped.Entry ?? new Entry(), customSerialization);
                }
                else
                {
                    throw new SerializationException($"Unsupported property {property.Name} on {instance.GetType().Name}!");
                }

                // Write new value to property
                if (property.CanWrite && !doNotUpdateProperty)
                {
                    mapped.Property.SetValue(instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Update or fill the collection using the mapping entry
        /// </summary>
        private static void UpdateCollection(object currentValue, Type memberType, ICustomAttributeProvider attributeProvider, Entry rootEntry,
            ICollectionStrategy strategy, ICustomSerialization customSerialization)
        {
            var currentCollection = currentValue as ICollection;

            // Loop over the collection and update the entries that are still present
            if (currentCollection != null)
            {
                foreach (var key in strategy.Keys())
                {
                    var item = strategy.ElementAt(key);
                    var match = rootEntry.SubEntries.Find(se => se.Identifier == key);
                    if (match == null)
                    {
                        strategy.Removed(key);
                        continue;
                    }

                    if (match.Value.Type < EntryValueType.Class)
                    {
                        item = CreatePrimitiveOrEnum(memberType, match.Value, customSerialization.FormatProvider);
                    }
                    else
                    {
                        UpdateInstance(item, match, customSerialization);
                    }
                    strategy.Updated(match, item);
                }
            }

            // Add new entries to the collection
            foreach (var subEntry in rootEntry.SubEntries.Where(se => se.Identifier == Entry.CreatedIdentifier))
            {
                object item;
                // All value types
                if (subEntry.Value.Type < EntryValueType.Class)
                {
                    // Create value type
                    item = CreatePrimitiveOrEnum(memberType, subEntry.Value, customSerialization.FormatProvider);
                }
                else
                {
                    // Create and update reference types
                    item = customSerialization.CreateInstance(memberType, attributeProvider, subEntry);
                    item = UpdateInstance(item, subEntry, customSerialization);
                }
                strategy.Added(subEntry, item);
            }

            // Finalize all operations
            strategy.Flush();
        }

        /// <summary>
        /// Create object for primitive values like int/string or enums
        /// </summary>
        private static object CreatePrimitiveOrEnum(Type propertyType, EntryValue entryValue, IFormatProvider formatProvider)
        {
            object item;
            // All value types
            if (entryValue.Type < EntryValueType.Enum)
            {
                // Create value type
                item = ToObject(entryValue.Type, entryValue.Current, formatProvider);
            }
            else
            {
                var elementType = ElementType(propertyType);
                item = ToObject(elementType, entryValue.Current, formatProvider);
            }
            return item;
        }

        /// <summary>
        /// Create strategy for collection
        /// </summary>
        private static ICollectionStrategy CreateStrategy(object collection, object currentCollection, 
            Type collectionType, ICustomSerialization serialization, ICustomAttributeProvider attributeProvider = null, 
            object instance = null)
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
                if(currentCollection is Array)
                {
                    strategy = new ArrayIListStrategy(
                        (IList)collection, serialization, attributeProvider, instance);
                }else
                    strategy = new ListStrategy((IList)collection, serialization);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported collection type {collectionType}");
            }

            return strategy;
        }

        /// <summary>
        /// Invoke a method on the target object using <see cref="DefaultSerialization"/>
        /// </summary>
        public static Entry InvokeMethod(object target, MethodEntry methodEntry)
        {
            return InvokeMethod(target, methodEntry, Serialization);
        }

        /// <summary>
        /// Invoke a method on the target object using custom customSerialization
        /// </summary>
        public static Entry InvokeMethod(object target, MethodEntry methodEntry, ICustomSerialization customSerialization)
        {
            var method = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == methodEntry.Name && (m.IsPublic || m.IsAssembly) && ParametersProvided(m.GetParameters(), methodEntry));
            var arguments = ConvertArguments(method, methodEntry, customSerialization);

            var result = method.Invoke(target, arguments);
            return result == null ? null : EncodeObject(result, customSerialization);
        }

        /// <summary>
        /// Check if all parameters are provided by the given <see cref="MethodEntry"/>
        /// </summary>
        private static bool ParametersProvided(ParameterInfo[] parameters, MethodEntry encodedMethod)
        {
            var encodedParameters = encodedMethod.Parameters.SubEntries;
            return parameters.All(p => encodedParameters.Any(se => se.Identifier == p.Name));
        }

        /// <summary>
        /// Convert encoded parameters to argument array for method invocation
        /// </summary>
        private static object[] ConvertArguments(MethodBase method, MethodEntry encodedMethod, ICustomSerialization customSerialization)
        {
            var parameters = method.GetParameters();
            var arguments = new object[parameters.Length];
            var parameterEntries = encodedMethod.Parameters.SubEntries;

            for (var i = 0; i < parameters.Length; i++)
            {
                var entry = parameterEntries[i];
                var parameter = parameters[i];
                var paramType = parameter.ParameterType;
                var argument = customSerialization.ConvertValue(paramType, parameter, entry, null);

                // Value types and strings do not need recursion
                if (ValueOrStringType(paramType))
                {
                }
                // Update collection from entry
                else if (argument is ICollection && entry != null)
                {
                    // Pick collection strategy
                    var strategy = CreateStrategy(argument, null, paramType, customSerialization);
                    UpdateCollection(null, paramType, parameter, entry, strategy, customSerialization);
                }
                // Update class
                else if (paramType.IsClass)
                {
                    if (entry == null)
                        UpdateInstance(argument, new Entry(), customSerialization);
                    else
                        UpdateInstance(argument, entry, customSerialization);
                }

                arguments[i] = argument;
            }

            return arguments;
        }

        #endregion

        /// <summary>
        /// Converts given value typed instance to a string with the given <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="formatProvider">Format provider used to convert the value to string</param>
        /// <returns></returns>
        internal static string ConvertToString(object value, IFormatProvider formatProvider)
        {
            var convertible = value as IConvertible;
            return convertible != null ? convertible.ToString(formatProvider) : value?.ToString();
        }
    }
}