using System;
using System.Collections.Generic;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Strategy interface passed into the serializers
    /// </summary>
    public interface ICustomSerialization
    {
        /// <summary>
        /// Gets or sets the <see cref="IFormatProvider"/> used or parsing and writing
        /// </summary>
        IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Filter the properties that shall be included in the conversion
        /// </summary>
        IEnumerable<PropertyInfo> GetProperties(Type sourceType);

        /// <summary>
        /// Types that shall be included as prototypes into the entry object
        /// </summary>
        EntryPrototype[] Prototypes(Type memberType, ICustomAttributeProvider attributeProvider);

        /// <summary>
        /// Possible values of a property
        /// </summary>
        /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
        /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider);

        /// <summary>
        /// Possible values for the elements of a collection property
        /// </summary>
        /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
        /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleElementValues(Type memberType, ICustomAttributeProvider attributeProvider);

        /// <summary>
        /// Entry validation object
        /// </summary>
        /// <param name="memberType">Either <see cref="PropertyInfo.PropertyType"/> or <see cref="ParameterInfo.ParameterType"/></param>
        /// <param name="attributeProvider">The <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/> to retrieve custom attributes</param>
        /// <returns>Validation object</returns>
        EntryValidation CreateValidation(Type memberType, ICustomAttributeProvider attributeProvider);

        /// <summary>
        /// Filter methods of a type that shall be serialized
        /// </summary>
        IEnumerable<MethodInfo> GetMethods(Type sourceType);

        /// <summary>
        /// Filter relevant constructors of an object
        /// </summary>
        IEnumerable<ConstructorInfo> GetConstructors(Type sourceType);
            /// <summary>
        /// Filter the properties that shall be included when creating the object
        /// </summary>
        IEnumerable<MappedProperty> WriteFilter(Type targetType, IEnumerable<Entry> entries);

        /// <summary>
        /// Get default value for a property without entry
        /// </summary>
        object DefaultValue(PropertyInfo property, object currentValue);

        /// <summary>
        /// Value of the property extracted from the entry
        /// </summary>
        object ConvertValue(Type memberType, ICustomAttributeProvider attributeProvider, Entry mappedEntry, object currentValue);

        /// <summary>
        /// Create instance of a given type using the entry for additional information
        /// </summary>
        object CreateInstance(Type elementType, Entry entry);

        /// <summary>
        /// Create instance of a collection item
        /// </summary>
        object CreateInstance(Type memberType, ICustomAttributeProvider attributeProvider, Entry encoded);

        /// <summary>
        /// Retrieves the unit type
        /// </summary>
        EntryUnitType GetUnitTypeByAttributes(ICustomAttributeProvider property);
    }

    /// <summary>
    /// Prototype object for a key
    /// </summary>
    public struct EntryPrototype
    {
        /// <summary>
        /// Create new entry prototype
        /// </summary>
        public EntryPrototype(string key, object prototype)
        {
            Key = key;
            Prototype = prototype;
        }

        /// <summary>
        /// Key of the prototype
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Object that represents a prototype
        /// </summary>
        public object Prototype { get; }
    }

    /// <summary>
    /// Struct that halds a property and its matching entry
    /// </summary>
    public struct MappedProperty
    {
        /// <summary>
        /// Property to convert
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Entry that maps to the property
        /// </summary>
        public Entry Entry { get; set; }
    }
}