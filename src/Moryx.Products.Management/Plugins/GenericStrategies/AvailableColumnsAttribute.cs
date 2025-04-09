// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Products.Model;
using Moryx.Serialization;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Attribute to select the entity column
    /// </summary>
    public class AvailableColumnsAttribute : PossibleValuesAttribute
    {
        private readonly Type _columnType;

        /// <summary>
        /// Create new instance of the <see cref="AvailableColumnsAttribute"/> without any
        /// type restriction
        /// </summary>
        public AvailableColumnsAttribute()
        {
        }

        /// <summary>
        /// Create new instance of the <see cref="AvailableColumnsAttribute"/> with a column
        /// type restriction
        /// </summary>
        public AvailableColumnsAttribute(Type columnType)
        {
            _columnType = columnType;
        }

        /// <inheritdoc />
        public override bool OverridesConversion => false;
        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IContainer container)
        {
            return typeof(IGenericColumns).GetProperties()
                .Where(p => _columnType == null || _columnType == p.PropertyType)
                .Select(p => p.Name)
                .OrderBy(p => p);
        }
    }
}
