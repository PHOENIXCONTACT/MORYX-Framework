using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.AbstractionLayer;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    internal sealed class TracingSerialization : DefaultSerialization
    {
        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            var filteredProperties = typeof(Tracing).GetProperties().Select(p => p.Name).ToArray();

            // Only simple properties not defined in the base
            return base.GetProperties(sourceType).Where(p => !filteredProperties.Contains(p.Name));
        }
    }
}
