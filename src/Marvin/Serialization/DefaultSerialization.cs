using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Tools;

namespace Marvin.Serialization
{
    /// <summary>
    /// Default implementation of serialization
    /// </summary>
    public class DefaultSerialization : ICustomSerialization
    {
        /// <see cref="ICustomSerialization"/>
        public virtual IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            return sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual EntryPrototype[] Prototypes(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            // Check if it is a list, array or dictionary
            if (EntryConvert.IsCollection(memberType))
            {
                memberType = EntryConvert.ElementType(memberType);
            }

            object prototype;
            // TODO: Maybe we find something better
            if (memberType == typeof(string))
            {
                prototype = string.Empty;
            }
            else
            {
                prototype = Activator.CreateInstance(memberType);
                ValueProviderExecutor.Execute(prototype, new ValueProviderExecutorSettings().AddDefaultValueProvider());
            }

            return new[]
            {
                new EntryPrototype(memberType.Name, prototype)
            };
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            // Element type for collections
            if (EntryConvert.IsCollection(memberType))
                return new[] { EntryConvert.ElementType(memberType).Name };

            // Names of Enums or null
            return memberType.IsEnum ? Enum.GetNames(memberType) : null;
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleElementValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var elementType = EntryConvert.ElementType(memberType);
            return PossibleValues(elementType, attributeProvider);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual EntryValidation CreateValidation(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var validation = new EntryValidation();

            //Determine if property is a password
            var passwordAttr = attributeProvider.GetCustomAttribute<PasswordAttribute>();
            if (passwordAttr != null)
                validation.IsPassword = true;

            var validationAttributes = attributeProvider.GetCustomAttributes<ValidationAttribute>();
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
        public virtual IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            return sourceType.GetMethods().Where(m => !m.IsSpecialName);
        }

        /// <see cref="ICustomSerialization"/>
        public virtual IEnumerable<ConstructorInfo> GetConstructors(Type sourceType)
        {
            return sourceType.GetConstructors();
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
        public virtual object ConvertValue(Type memberType, ICustomAttributeProvider attributeProvider, Entry mappedEntry, object currentValue)
        {
            // Other operations depend on the element type
            switch (mappedEntry.Value.Type)
            {
                case EntryValueType.Class:
                    return currentValue ?? Activator.CreateInstance(memberType);
                case EntryValueType.Collection:
                    return CollectionBuilder(memberType, currentValue, mappedEntry);
                default:
                    var value = mappedEntry.Value.Current;
                    return value == null ? null : EntryConvert.ToObject(memberType, value);
            }
        }
        /// <see cref="ICustomSerialization"/>
        public virtual object CreateInstance(Type memberType, ICustomAttributeProvider attributeProvider, Entry encoded)
        {
            if (EntryConvert.IsCollection(memberType))
            {
                memberType = EntryConvert.ElementType(memberType);
            }
            return CreateInstance(memberType, encoded);
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