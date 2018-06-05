using System;
using System.Collections.Generic;

namespace Marvin.Configuration
{
    /// <summary>
    /// Base attribute for all attributes that support multiple values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PossibleValuesAttribute : Attribute
    {
        /// <summary>
        /// All possible values for this member represented as strings. 
        /// </summary>
        public abstract IEnumerable<string> ResolvePossibleValues();

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public abstract bool OverridesConversion { get; }

        /// <summary>
        /// Flag if new values shall be updated from the old value
        /// </summary>
        public abstract bool UpdateFromPredecessor { get; }

        /// <summary>
        /// String to value conversion
        /// </summary>
        public virtual object ConvertToConfigValue(string value)
        {
            return value;
        }
    }
}
