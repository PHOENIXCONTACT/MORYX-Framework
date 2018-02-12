using System.Collections;
using System.Collections.Generic;
using Marvin.Container;

namespace Marvin.Configuration
{
    /// <summary>
    /// Attribute to enrich a config file with possible values for a property.
    /// </summary>
    public class PrimitiveValuesAttribute : PossibleConfigValuesAttribute
    {
        private string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params string[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params byte[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params int[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params long[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params double[] values)
        {
            GenerateStringValues(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public PrimitiveValuesAttribute(params bool[] values)
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

        /// <inheritdoc />
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            return _values;
        }

        /// <inheritdoc />
        public override bool OverridesConversion => false;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => true;
    }
}