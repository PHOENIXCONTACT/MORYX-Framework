using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Attribute for hard coded valid product values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PossibleProductValuesAttribute : Attribute
    {
        /// <summary>
        /// Initialize the attribute with valid values
        /// </summary>
        public PossibleProductValuesAttribute(params string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// All values valid for this property
        /// </summary>
        public string[] Values { get; protected set; }
    }
}