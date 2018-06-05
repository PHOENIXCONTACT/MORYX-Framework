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
        /// Filter the properties that shall be included in the conversion
        /// </summary>
        IEnumerable<PropertyInfo> ReadFilter(Type sourceType);

        /// <summary>
        /// Types that shall be included as prototypes into the entry object
        /// </summary>
        EntryPrototype[] Prototypes(PropertyInfo property);

        /// <summary>
        /// Possible values of a property
        /// </summary>
        /// <param name="property">Property to check</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleValues(PropertyInfo property);

        /// <summary>
        /// Possible values of a property
        /// </summary>
        /// <param name="parameter">A method parameter that shall be serialized</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleValues(ParameterInfo parameter);

        /// <summary>
        /// Possible values for the elements of a collection property
        /// </summary>
        /// <param name="property">Property to check</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleElementValues(PropertyInfo property);

        /// <summary>
        /// Possible values for the elements of a collection parameter
        /// </summary>
        /// <param name="parameter">Parameter to check</param>
        /// <returns>Possible values or null</returns>
        string[] PossibleElementValues(ParameterInfo parameter);

        /// <summary>
        /// Entry validation object
        /// </summary>
        /// <param name="property">Property to read validation for</param>
        /// <returns>Validation object</returns>
        EntryValidation CreateValidation(PropertyInfo property);

        /// <summary>
        /// Filter methods of a type that shall be serialized
        /// </summary>
        IEnumerable<MethodInfo> MethodFilter(Type sourceType);

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
        object PropertyValue(PropertyInfo property, Entry mappedEntry, object currentValue);

        /// <summary>
        /// Convert the value of a method parameter from the entry representing it.
        /// </summary>
        object ParameterValue(ParameterInfo parameter, Entry mappedEntry);


        /// <summary>
        /// Create instance of a collection item
        /// </summary>
        object CreateInstance(MappedProperty mappedRoot, Entry encoded);

        /// <summary>
        /// Create instance of a given type using the entry for additional information
        /// </summary>
        object CreateInstance(Type elementType, Entry entry);
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