// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Non-generated part of the customSerialization helper
/// </summary>
public static partial class EntryConvert
{
    /// <summary>
    /// Default strategy instance
    /// </summary>
    private static readonly ICustomSerialization _serialization = new DefaultSerialization();

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
    /// <returns>Converted property</returns>
    public static Entry EncodeProperty(PropertyInfo property)
    {
        return EncodeProperty(property, _serialization, null);
    }

    /// <summary>
    /// Convert a single property into a derived type of entry using a custom strategy
    /// </summary>
    /// <returns>Converted property</returns>
    public static Entry EncodeProperty(PropertyInfo property, ICustomSerialization customSerialization) => EncodeProperty(property, customSerialization, null);

    /// <summary>
    /// Convert a single property into a derived type of entry using a custom strategy
    /// </summary>
    /// <returns>Converted property</returns>
    public static Entry EncodeProperty(PropertyInfo property, ICustomSerialization customSerialization, object instance)
    {
        var converter = customSerialization.GetConverter(property.PropertyType, property);
        if (converter == null)
        {
            return DefaultPropertyEncoding(property, customSerialization, instance);
        }

        if (!TryGetValue(property, instance, out var value))
        {
            var faulty = DefaultPropertyEncoding(property, customSerialization, instance);
            faulty.Value.Type = EntryValueType.Exception;
            faulty.Value.Current = value.ToString();
            return faulty;
        }

        return EncodePropertyWith(converter, customSerialization, property, instance, value);
    }

    /// <summary>
    /// Convert a single property without <see cref="EntryConverter"/>
    /// </summary>
    private static Entry DefaultPropertyEncoding(PropertyInfo property, ICustomSerialization customSerialization, object instance = default)
    {
        // Fill with default if entry is null
        var entry = CreateFromProperty(property, customSerialization);
        entry.Value = CreateEntryValue(property, customSerialization, instance);

        // No prototypes for readonly entries or entries without flexible sub-entries
        if (entry.Value.IsReadOnly || entry.Value.Type != EntryValueType.Collection && entry.Value.Type != EntryValueType.Class)
        {
            return entry;
        }

        // Determine and convert prototypes
        var prototypes = Prototypes(property.PropertyType, property, customSerialization, instance);
        entry.Prototypes.AddRange(prototypes);

        return entry;
    }

    private static Entry CreateFromProperty(PropertyInfo property, ICustomSerialization serialization) => new Entry
    {
        Identifier = property.Name,
        DisplayName = property.GetDisplayName() ?? property.Name,
        Description = property.GetDescription(),
        Validation = serialization.CreateValidation(property.PropertyType, property)
    };

    /// <see cref="ICustomSerialization"/>
    private static EntryValue CreateEntryValue(PropertyInfo property, ICustomSerialization customSerialization, object instance = null)
    {
        // Prepare object
        var entryValue = new EntryValue
        {
            Type = TransformType(property.PropertyType),
            UnitType = customSerialization.GetUnitTypeByAttributes(property),
            IsReadOnly = IsReadOnly(property),
            Possible = customSerialization.PossibleValues(property.PropertyType, property, instance)
        };

        // Get most basic default
        var defaultAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultAttribute != null)
            entryValue.Default = defaultAttribute.Value.ToString();
        else if (entryValue.Possible != null && entryValue.Possible.Length >= 1)
            entryValue.Default = entryValue.Possible[0].Key;
        else if (property.PropertyType.IsValueType && entryValue.Type != EntryValueType.Struct)
        {
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            var defaultInstance = Activator.CreateInstance(underlyingType ?? property.PropertyType);
            entryValue.Default = ConvertToString(defaultInstance, customSerialization.FormatProvider);
        }
        // Value types should have the default value as current value (except decomposed structs)
        if (ValueOrStringType(property.PropertyType) && entryValue.Type != EntryValueType.Struct)
            entryValue.Current = ConvertToString(entryValue.Default, customSerialization.FormatProvider);

        return entryValue;
    }

    private static bool IsReadOnly(PropertyInfo property)
    {
        if (!property.CanWrite)
        {
            return true;
        }

        var readOnlyAtt = property.GetCustomAttribute<ReadOnlyAttribute>();
        return readOnlyAtt?.IsReadOnly ?? false;
    }

    /// <summary>
    /// Create prototypes for possible values of an entry
    /// </summary>
    private static IEnumerable<Entry> Prototypes(Type memberType, ICustomAttributeProvider customAttributeProvider, ICustomSerialization customSerialization, object instance = null)
    {
        EntryPossible[] possibleElementValues;
        if (IsCollection(memberType))
        {
            possibleElementValues = customSerialization.PossibleElementValues(memberType, customAttributeProvider);
        }
        else
        {
            possibleElementValues = customSerialization.PossibleValues(memberType, customAttributeProvider, instance);
        }

        var validation = customSerialization.CreateValidation(memberType, customAttributeProvider);
        foreach (var prototype in customSerialization.Prototypes(memberType, customAttributeProvider))
        {
            var prototypeEntry = Prototype(prototype, customSerialization);
            prototypeEntry.Identifier = prototype.Key;
            prototypeEntry.Validation = validation;
            prototypeEntry.Value.Possible = possibleElementValues;
            yield return prototypeEntry;
        }
    }

    private static bool TryGetValue(PropertyInfo property, object instance, out object value)
    {
        try
        {
            value = instance != null ? property.GetValue(instance) : null;
            return true;
        }
        catch (Exception ex)
        {
            value = ex;
            return false;
        }
    }

    /// <summary>
    /// Encode a property using an <see cref="EntryConverter"/>
    /// </summary>
    private static Entry EncodePropertyWith(EntryConverter converter, ICustomSerialization customSerialization,
        PropertyInfo property, object instance = default, object value = default)
    {
        var context = new EntryConverterContext(property.PropertyType, property, customSerialization, instance);
        var encoded = EncodeWith(converter, context, value, CreateFromProperty(property, customSerialization));
        encoded.Value.IsReadOnly |= IsReadOnly(property);

        return encoded;
    }

    private static Entry EncodeWith(EntryConverter converter, EntryConverterContext context, object value, Entry metadata)
    {
        var encoded = converter.Encode(value, context);

        encoded.Identifier = metadata.Identifier;
        encoded.DisplayName = metadata.DisplayName;
        encoded.Description = metadata.Description;
        encoded.Validation = metadata.Validation;

        return encoded;
    }

    /// <summary>
    /// Create a prototype entry for a certain type using <see cref="DefaultSerialization"/>
    /// </summary>
    public static Entry Prototype(EntryPrototype prototype) => Prototype(prototype, _serialization);

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
        return EncodeClass(objType, _serialization);
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
            // Converters encode the complete entry
            var converter = customSerialization.GetConverter(property.PropertyType, property);
            if (converter is not null)
            {
                encodedClass.SubEntries.Add(EncodePropertyWith(converter, customSerialization, property));
                continue;
            }

            var converted = DefaultPropertyEncoding(property, customSerialization);
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
        return EncodeObject(instance, _serialization);
    }

    /// <summary>
    /// Convert object instance using custom strategy
    /// </summary>
    public static Entry EncodeObject(object instance, ICustomSerialization customSerialization)
    {
        var instanceType = instance.GetType();

        // Converters encode the complete entry
        var instanceConverter = customSerialization.GetConverter(instanceType, instanceType);
        if (instanceConverter is not null)
        {
            return EncodeConvertedInstance(instanceConverter, customSerialization, instance);
        }

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
            //exclude property that can lead to self reference
            var propertyType = property.PropertyType;
            if (propertyType == instanceType)
                continue;

            var isFaulty = !TryGetValue(property, instance, out var value);

            // Converters encode the complete entry
            var converter = customSerialization.GetConverter(propertyType, property);
            if (converter is not null && !isFaulty)
            {
                converted.SubEntries.Add(EncodePropertyWith(converter, customSerialization, property, instance, value));
                continue;
            }

            var convertedProperty = DefaultPropertyEncoding(property, customSerialization, instance);
            // Change type in case of exception
            if (isFaulty)
                convertedProperty.Value.Type = EntryValueType.Exception;

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
                    convertedProperty.Value.Current = (value as Exception)?.ToString() ?? string.Empty;
                    break;
                case EntryValueType.Stream:
                    if (value is Stream stream)
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
    /// Encode an object instance using an <see cref="EntryConverter"/> for its type
    /// </summary>
    private static Entry EncodeConvertedInstance(EntryConverter converter, ICustomSerialization customSerialization, object instance)
    {
        var instanceType = instance.GetType();
        var context = new EntryConverterContext(instanceType, instanceType, customSerialization, null);
        return EncodeWith(converter, context, instance, CreateFromType(instanceType, customSerialization));
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
        stream.ReadExactly(bytes, 0, (int)stream.Length);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Encode a <see cref="MethodBase"/> to the transmittable <see cref="MethodEntry"/>
    /// using <see cref="DefaultSerialization"/>
    /// </summary>
    /// <param name="method">Method to encode</param>
    /// <returns>Encoded method entry</returns>
    public static MethodEntry EncodeMethod(MethodBase method)
    {
        return EncodeMethod(method, _serialization, null);
    }

    /// <summary>
    /// Encode a <see cref="MethodBase"/> to the transmittable <see cref="MethodEntry"/>
    /// </summary>
    public static MethodEntry EncodeMethod(MethodBase method, ICustomSerialization serialization) => EncodeMethod(method, serialization, null);

    /// <summary>
    /// Encode a <see cref="MethodBase"/> to the transmittable <see cref="MethodEntry"/>
    /// </summary>
    public static MethodEntry EncodeMethod(MethodBase method, ICustomSerialization serialization, object instance)
    {
        var methodEntry = new MethodEntry
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
                SubEntries = method.GetParameters().Select(p => ConvertParameter(p, serialization, instance)).ToList()
            }
        };

        // Constructors have no return type
        if (method is MethodInfo methodInfo)
        {
            methodEntry.IsAsync = IsAsyncMethod(methodInfo);
        }

        return methodEntry;
    }

    /// <summary>
    /// Encode all methods of an object using <see cref="DefaultSerialization"/>
    /// </summary>
    public static IEnumerable<MethodEntry> EncodeMethods(object source)
    {
        return EncodeMethods(source, _serialization);
    }

    /// <summary>
    /// Encode all methods of an object using a custom customSerialization
    /// </summary>
    public static IEnumerable<MethodEntry> EncodeMethods(object source, ICustomSerialization serialization)
    {
        var methods = serialization.GetMethods(source.GetType());
        return methods.Select(m => EncodeMethod(m, serialization, source));
    }

    /// <summary>
    /// Encode all methods of class using <see cref="DefaultSerialization"/>
    /// </summary>
    public static IEnumerable<MethodEntry> EncodeMethods(Type objType)
    {
        return EncodeMethods(objType, _serialization);
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
        return EncodeConstructors(objType, _serialization);
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
    private static Entry ConvertParameter(ParameterInfo parameter, ICustomSerialization serialization, object instance = null)
    {
        var parameterType = parameter.ParameterType;

        // Converters encode the complete entry
        var converter = serialization.GetConverter(parameterType, parameter);
        if (converter is not null)
        {
            var context = new EntryConverterContext(parameterType, parameter, serialization, instance);
            return EncodeWith(converter, context, DefaultArgument(parameter), CreateFromParameter(parameter, serialization));
        }

        var defaultValue = DefaultArgument(parameter)?.ToString();
        var parameterModel = CreateFromParameter(parameter, serialization);
        parameterModel.Value = new EntryValue
        {
            Type = TransformType(parameter.ParameterType),
            UnitType = serialization.GetUnitTypeByAttributes(parameter),
            Current = defaultValue,
            Default = defaultValue,
            Possible = serialization.PossibleValues(parameterType, parameter, instance)
        };

        switch (parameterModel.Value.Type)
        {
            case EntryValueType.Class:
                parameterModel.Value.Current = parameterType.Name;
                parameterModel.Prototypes.AddRange(Prototypes(parameterType, parameter, serialization, instance));
                parameterModel.SubEntries = EncodeClass(parameterType, serialization).SubEntries;
                break;
            case EntryValueType.Collection:
                var elemType = ElementType(parameterType);
                parameterModel.Value.Current = elemType.Name;
                parameterModel.Prototypes.AddRange(Prototypes(parameterType, parameter, serialization, instance));
                break;
        }

        return parameterModel;
    }

    private static object DefaultArgument(ParameterInfo parameter) =>
        parameter.HasDefaultValue ? parameter.DefaultValue : null;

    private static Entry CreateFromParameter(ParameterInfo parameter, ICustomSerialization serialization)
    {
        var entry = new Entry
        {
            Identifier = parameter.Name,
            DisplayName = parameter.GetDisplayName() ?? parameter.Name,
            Description = parameter.GetDescription(),
            Validation = serialization.CreateValidation(parameter.ParameterType, parameter)
        };

        // mark the parameter as required when there is no default value
        entry.Validation.IsRequired = !parameter.HasDefaultValue;

        return entry;
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
        UpdateInstance(instance, encoded, _serialization);
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
        return CreateInstance(type, encoded, _serialization);
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
        return CreateInstance(type, encodedConstructor, _serialization);
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
        return UpdateInstance(instance, encoded, _serialization);
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

            // Converters decode the complete entry, members without entry keep their current value
            var converter = customSerialization.GetConverter(propertyType, property);
            if (converter is not null)
            {
                if (mapped.Entry is not null && property.CanWrite)
                {
                    var context = new EntryConverterContext(propertyType, property, customSerialization, instance);
                    property.SetValue(instance, converter.Decode(mapped.Entry, currentValue, context));
                }
                continue;
            }

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
                throw new SerializationException($"Unsupported property {property.Name} on {instance.GetType().Name}.");
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
        foreach (var subEntry in rootEntry.SubEntries.Where(se => se.Identifier.ToLower().StartsWith(Entry.CreatedIdentifier.ToLower())))
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
            if (currentCollection is Array)
            {
                strategy = new ArrayIListStrategy(
                    (IList)collection, serialization, attributeProvider, instance);
            }
            else
            {
                strategy = new ListStrategy((IList)collection, serialization);
            }
        }
        else
        {
            throw new InvalidOperationException($"Unsupported collection type {collectionType}.");
        }

        return strategy;
    }

    /// <summary>
    /// Invoke a method on the target object using <see cref="DefaultSerialization"/>
    /// </summary>
    public static Entry InvokeMethod(object target, MethodEntry methodEntry)
    {
        return InvokeMethod(target, methodEntry, _serialization);
    }

    /// <summary>
    /// Invoke a method on the target object using custom customSerialization
    /// Async methods are executed asynchronous
    /// </summary>
    public static Task<Entry> InvokeMethodAsync(object target, MethodEntry methodEntry, CancellationToken cancellationToken = default)
    {
        return InvokeMethodAsync(target, methodEntry, _serialization, cancellationToken);
    }

    /// <summary>
    /// Invoke a method on the target object using custom customSerialization
    /// Async methods are executed asynchronous
    /// </summary>
    public static async Task<Entry> InvokeMethodAsync(object target, MethodEntry methodEntry, ICustomSerialization customSerialization,
        CancellationToken cancellationToken = default)
    {
        var method = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == methodEntry.Name && (m.IsPublic || m.IsAssembly) && ParametersProvided(m.GetParameters(), methodEntry));
        if (method == null)
        {
            throw new MissingMethodException($"Method {methodEntry.Name} not found on target {target.GetType().Name}.");
        }

        var arguments = ConvertArguments(method, methodEntry, customSerialization, target);
        object invokeResult = null;

        // Also part of MethodEntry but be safer than sorry
        var isAsync = IsAsyncMethod(method);
        if (isAsync)
        {
            // TODO check cancellationToken usage and pass it to invoked method if possible
            var task = (Task)method.Invoke(target, arguments)!;
            await task;

            var returnType = method.ReturnType;
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // Extract T from Task<T>
                var resultProperty = returnType.GetProperty(nameof(Task<>.Result));
                invokeResult = resultProperty!.GetValue(task);
            }
        }
        else
        {
            invokeResult = method.Invoke(target, arguments);
        }

        return invokeResult == null ? null : EncodeObject(invokeResult, customSerialization);
    }

    /// <summary>
    /// Invoke a method on the target object using custom customSerialization
    /// Async methods are synchronized
    /// </summary>
    public static Entry InvokeMethod(object target, MethodEntry methodEntry, ICustomSerialization customSerialization)
    {
        return InvokeMethodAsync(target, methodEntry, customSerialization).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Checks if the return type of the method is a task
    /// </summary>
    private static bool IsAsyncMethod(MethodInfo method) =>
        typeof(Task).IsAssignableFrom(method.ReturnType);

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
    private static object[] ConvertArguments(MethodBase method, MethodEntry encodedMethod, ICustomSerialization customSerialization,
        object instance = null)
    {
        var parameters = method.GetParameters();
        var arguments = new object[parameters.Length];
        var parameterEntries = encodedMethod.Parameters.SubEntries;

        for (var i = 0; i < parameters.Length; i++)
        {
            var entry = parameterEntries[i];
            var parameter = parameters[i];
            var paramType = parameter.ParameterType;

            // Converters decode the complete entry, rejected values fall back to the parameter default
            var converter = customSerialization.GetConverter(paramType, parameter);
            if (converter is not null)
            {
                var context = new EntryConverterContext(paramType, parameter, customSerialization, instance);
                arguments[i] = converter.Decode(entry, DefaultArgument(parameter), context);
                continue;
            }

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

}
