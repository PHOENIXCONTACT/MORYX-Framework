using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Container;
using Marvin.Tools;

namespace Marvin.Configuration
{
    /// <summary>
    /// <see cref="PossibleValuesAttribute"/> to provide possible types by an array or base type
    /// </summary>
    public class PossibleTypesAttribute : PossibleValuesAttribute
    {
        private readonly Type[] _types;

        /// <summary>
        /// Creates an new instance of <see cref="PossibleTypesAttribute"/>
        /// Searches for all public implementations of the given base types.
        /// </summary>
        public PossibleTypesAttribute(Type baseType)
        {
            _types = ReflectionTool.GetPublicClasses(baseType);
        }

        /// <summary>
        /// Creates an new instance of <see cref="PossibleTypesAttribute"/>
        /// Uses the types of the argument.
        /// </summary>
        public PossibleTypesAttribute(Type[] types)
        {
            _types = types;
        }

        /// <inheritdoc />
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            return _types.Select(t => t.Name);
        }

        /// <inheritdoc />
        public override object ConvertToConfigValue(IContainer container, string value)
        {
            var type = _types.Single(t => t.Name == value);
            return Activator.CreateInstance(type);
        }

        /// <inheritdoc />
        public override bool OverridesConversion => true;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;
    }
}
