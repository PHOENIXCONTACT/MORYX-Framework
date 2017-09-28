using System.Collections;
using System.Collections.Generic;
using Marvin.Testing;

namespace Marvin.Runtime.Configuration
{
    /// <summary>
    /// Attribute to enrich a config file with possible values for a property.
    /// </summary>
    [OpenCoverIgnore]
    public class PossibleValuesAttribute : PossibleConfigValuesAttribute
    {
        private string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params string[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params byte[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params int[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params long[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params double[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PossibleValuesAttribute(params bool[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Generates string values from the given list of typed values.
        /// </summary>
        /// <param name="values">The values.</param>
        private void GenerateStringValues(IEnumerable values)
        {
            var stringValues = new List<string>();
            foreach (var value in values)
            {
                stringValues.Add(value.ToString());
            }
            _values = stringValues.ToArray();
        }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        /// <param name="pluginContainer"></param>
        /// <returns></returns>
        public override IEnumerable<string> ResolvePossibleValues(Marvin.Container.IContainer pluginContainer)
        {
            return _values;
        }

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public override bool OverridesConversion
        {
            get { return false; }
        }

        /// <summary>
        /// Flag if new values shall be updated from the old value
        /// </summary>
        public override bool UpdateFromPredecessor
        {
            get { return true; }
        }
    }
}
