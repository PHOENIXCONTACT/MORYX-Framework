using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Marvin.Configuration;

namespace Marvin.Serialization
{
    /// <summary>
    /// Default implementation of serialization
    /// </summary>
    public class DefaultSerialization : ICustomSerialization
    {
        /// <see cref="ICustomSerialization"/>
        public virtual IEnumerable<PropertyInfo> ReadFilter(Type sourceType)
        {
            return sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual EntryPrototype[] Prototypes(PropertyInfo property)
        {
            var elementType = property.PropertyType;
            // Check if it is a list, array or dictionary
            if (EntryConvert.IsCollection(property.PropertyType))
            {
                elementType = EntryConvert.ElementType(elementType);
            }

            object prototype;
            // TODO: Maybe we find something better
            if (elementType == typeof(string))
            {
                prototype = string.Empty;
            }
            else
            {
                prototype = Activator.CreateInstance(elementType);
                ValueProviderExecutor.Execute(prototype, new ValueProviderExecutorSettings().AddDefaultValueProvider());
            }

            return new[]
            {
                new EntryPrototype(elementType.Name, prototype)
            };
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleValues(PropertyInfo property)
        {
            return PossibleValues(property.PropertyType);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleValues(ParameterInfo parameter)
        {
            return PossibleValues(parameter.ParameterType);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleElementValues(PropertyInfo property)
        {
            var elementType = EntryConvert.ElementType(property.PropertyType);
            return PossibleValues(elementType);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleElementValues(ParameterInfo parameter)
        {
            var elementType = EntryConvert.ElementType(parameter.ParameterType);
            return PossibleValues(elementType);
        }

        /// <summary>
        /// Determine possible values of a type
        /// </summary>
        protected virtual string[] PossibleValues(Type serializedType)
        {
            // Element type for collections
            if (EntryConvert.IsCollection(serializedType))
                return new[] { EntryConvert.ElementType(serializedType).Name };

            // Names of Enums or null
            return serializedType.IsEnum ? Enum.GetNames(serializedType) : null;
        }

        /// <see cref="ICustomSerialization"/>
        public virtual EntryValidation CreateValidation(PropertyInfo property)
        {
            var validation = new EntryValidation();

            //Determine if property is a password
            var passwordAttr = property.GetCustomAttribute<PasswordAttribute>();
            if (passwordAttr != null)
                validation.IsPassword = true;

            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>().ToArray();
            if (validationAttributes.Length == 0)
                return validation;

            // Iterate over attributes reading all validation rules
            foreach (var attribute in validationAttributes)
            {
                if (attribute is MinLengthAttribute)
                    validation.MinLenght = ((MinLengthAttribute)attribute).Length;
                else if (attribute is MaxLengthAttribute)
                    validation.MaxLenght = ((MaxLengthAttribute)attribute).Length;
                else if (attribute is RegularExpressionAttribute)
                    validation.Regex = ((RegularExpressionAttribute)attribute).Pattern;
                else if (attribute is StringLengthAttribute)
                {
                    var strLength = (StringLengthAttribute)attribute;
                    validation.MinLenght = strLength.MinimumLength;
                    validation.MaxLenght = strLength.MaximumLength;
                }
                else if (attribute is RequiredAttribute)
                    validation.IsRequired = true;
            }

            return validation;
        }

        /// <see cref="ICustomSerialization"/>
        public virtual IEnumerable<MethodInfo> MethodFilter(Type sourceType)
        {
            return sourceType.GetMethods().Where(m => !m.IsSpecialName);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
        {
            // Return pairs where available
            return from entry in encoded
                   let property = sourceType.GetProperty(entry.Key.Identifier)
                   select new MappedProperty
                   {
                       Entry = entry,
                       Property = property
                   };
        }

        /// <see cref="ICustomSerialization"/>
        public virtual object DefaultValue(PropertyInfo property, object currentValue)
        {
            // No default is better than the current value
            if (currentValue != null)
                return currentValue;

            // Try to read default from attribute
            var defaultAtt = property.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAtt != null)
                return defaultAtt.Value;

            // For arrays create empty element array
            if (property.PropertyType.IsArray)
                return Array.CreateInstance(property.PropertyType.GetElementType(), 0);

            // In all other cases use the activator
            return Activator.CreateInstance(property.PropertyType);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual object PropertyValue(PropertyInfo property, Entry mappedEntry, object currentValue)
        {
            var propertyType = property.PropertyType;
            // Other operations depend on the element type
            switch (mappedEntry.Value.Type)
            {
                case EntryValueType.Class:
                    return currentValue ?? Activator.CreateInstance(propertyType);
                case EntryValueType.Collection:
                    return CollectionBuilder(propertyType, currentValue, mappedEntry);
                default:
                    var value = mappedEntry.Value.Current;
                    return value == null ? null : EntryConvert.ToObject(propertyType, value);
            }
        }

        /// <see cref="ICustomSerialization"/>
        public virtual object ParameterValue(ParameterInfo parameter, Entry mappedEntry)
        {
            var parameterType = parameter.ParameterType;
            // Other operations depend on the element type
            switch (mappedEntry.Value.Type)
            {
                case EntryValueType.Class:
                    return Activator.CreateInstance(parameterType);
                case EntryValueType.Collection:
                    return CollectionBuilder(parameterType, null, mappedEntry);
                default:
                    var value = mappedEntry.Value.Current;
                    return value == null ? null : EntryConvert.ToObject(parameterType, value);
            }
        }

        /// <see cref="ICustomSerialization"/>
        public virtual object CreateInstance(MappedProperty mappedRoot, Entry encoded)
        {
            var elemType = mappedRoot.Property.PropertyType;
            if (EntryConvert.IsCollection(elemType))
            {
                elemType = EntryConvert.ElementType(elemType);
            }
            return CreateInstance(elemType, encoded);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual object CreateInstance(Type elementType, Entry entry)
        {
            return Activator.CreateInstance(elementType);
        }

        /// <summary>
        /// Build collection object from entry
        /// </summary>
        protected static object CollectionBuilder(Type collectionType, object currentValue, Entry collectionEntry)
        {
            // Arrays must be recreated whenever their size changes
            if (collectionType.IsArray)
            {
                var currentArray = (Array)currentValue;
                var size = collectionEntry.SubEntries.Count;
                return currentArray != null && currentArray.Length == size
                    ? currentArray : Array.CreateInstance(collectionType.GetElementType(), size);
            }

            // Create instance for collections of type Dictionary
            if (collectionType.GetGenericArguments().Length == 2) // TODO: Better criteria for dictionary
            {
                // Use dictionary when interface where used
                if (collectionType.IsInterface)
                    collectionType = typeof(Dictionary<,>).MakeGenericType(collectionType.GenericTypeArguments);
                // Reuse current object if available
                return currentValue ?? Activator.CreateInstance(collectionType);
            }

            // Create instance for collections of type list
            if (typeof(IEnumerable).IsAssignableFrom(collectionType))
            {
                // Use lists when interfaces where used
                if (collectionType.IsInterface)
                    collectionType = typeof(List<>).MakeGenericType(collectionType.GenericTypeArguments);
                // Reuse current object if available
                return currentValue ?? Activator.CreateInstance(collectionType);
            }


            // Other collections are not supported
            return null;
        }
    }
}