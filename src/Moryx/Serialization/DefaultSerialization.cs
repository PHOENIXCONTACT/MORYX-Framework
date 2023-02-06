// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moryx.Configuration;
using Moryx.Tools;

namespace Moryx.Serialization
{
    /// <summary>
    /// Default implementation of serialization
    /// </summary>
    public class DefaultSerialization : ICustomSerialization
    {
        /// <summary>
        /// Constructor to construct a <see cref="DefaultSerialization"/> instance
        /// </summary>
        public DefaultSerialization()
        {
            FormatProvider = Thread.CurrentThread.CurrentCulture;
        }

        /// <inheritdoc />
        public IFormatProvider FormatProvider { get; set; }

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

            List<EntryPrototype> prototypes = new List<EntryPrototype>();
            if (memberType == typeof(string))
            {
                prototypes.Add(new EntryPrototype(nameof(String), string.Empty));
            }
            else if (memberType.IsEnum)
            {
                foreach (Enum enumValue in Enum.GetValues(memberType))
                    prototypes.Add(new EntryPrototype(enumValue.ToString("G"), enumValue));
            }
            else
            {
                var prototype = Activator.CreateInstance(memberType);
                if (memberType.IsClass)
                    ValueProviderExecutor.Execute(prototype, new ValueProviderExecutorSettings().AddDefaultValueProvider());
                prototypes.Add(new EntryPrototype(memberType.Name, prototype));
            }

            return prototypes.ToArray();
        }

        /// <see cref="ICustomSerialization"/>
        public virtual string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            // Element type for collections
            var isCollection = EntryConvert.IsCollection(memberType);
            if (isCollection)
                memberType = EntryConvert.ElementType(memberType);

            // Enum names, member name or null
            return memberType.IsEnum
                ? Enum.GetNames(memberType)
                : isCollection ? new[] { memberType.Name } : null;
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

            var validationAttributes = attributeProvider.GetCustomAttributes<ValidationAttribute>();
            if (validationAttributes.Length == 0)
                return validation;

            // Iterate over attributes reading all validation rules
            foreach (var attribute in validationAttributes)
            {
                if (attribute is MinLengthAttribute minAttribute)
                {
                    validation.Minimum = minAttribute.Length;
                }
                else if (attribute is MaxLengthAttribute maxAttribute)
                {
                    validation.Maximum = maxAttribute.Length;
                }
                else if (attribute is RangeAttribute rangeAttribute)
                {
                    validation.Minimum = Convert.ToDouble(rangeAttribute.Minimum);
                    validation.Maximum = Convert.ToDouble(rangeAttribute.Maximum);
                }
                else if (attribute is RegularExpressionAttribute regexAttribute)
                    validation.Regex = regexAttribute.Pattern;
                else if (attribute is StringLengthAttribute strLength)
                {
                    validation.Minimum = strLength.MinimumLength;
                    validation.Maximum = strLength.MaximumLength;
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
                   let property = sourceType.GetProperty(entry.Identifier)
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
                case EntryValueType.Stream:
                    Stream targetStream;

                    var safeContent = mappedEntry.Value.Current ?? "";
                    var contentBytes = Convert.FromBase64String(safeContent);
                    var currentStream = currentValue as Stream;

                    var createNewMemoryStream = currentStream == null || !currentStream.CanWrite;
                    if (!createNewMemoryStream &&
                        currentStream.GetType() == typeof(MemoryStream) &&
                        currentStream.Length < contentBytes.Length)
                    {
                        // MemoryStream is not expandable
                        createNewMemoryStream = true;
                    }

                    if (currentStream != null && !createNewMemoryStream)
                    {
                        if (currentStream.CanSeek)
                            currentStream.Seek(0, SeekOrigin.Begin);

                        targetStream = currentStream;
                        targetStream.Write(contentBytes, 0, contentBytes.Length);
                        targetStream.SetLength(contentBytes.Length);
                    }
                    else
                    {
                        currentStream?.Dispose();

                        targetStream = new MemoryStream(contentBytes);
                    }

                    return targetStream;

                case EntryValueType.Class:
                    return currentValue ?? Activator.CreateInstance(memberType);
                case EntryValueType.Collection:
                    return CollectionBuilder(memberType, currentValue, mappedEntry);
                default:
                    var value = mappedEntry.Value.Current;
                    return value == null ? null : EntryConvert.ToObject(memberType, value, FormatProvider);
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

        /// <inheritdoc />
        public EntryUnitType GetUnitTypeByAttributes(ICustomAttributeProvider property)
        {
            var unitType = EntryUnitType.None;

            var passwordAttr = property.GetCustomAttribute<PasswordAttribute>();
            if (passwordAttr != null)
                unitType = EntryUnitType.Password;

            var fileAttr = property.GetCustomAttribute<FileSystemPathAttribute>();
            if (fileAttr != null)
            {
                switch (fileAttr.Type)
                {
                    case FileSystemPathType.File:
                        unitType = EntryUnitType.File;
                        break;
                    case FileSystemPathType.Directory:
                        unitType = EntryUnitType.Directory;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return unitType;
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
            if (EntryConvert.IsDictionary(collectionType))
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
