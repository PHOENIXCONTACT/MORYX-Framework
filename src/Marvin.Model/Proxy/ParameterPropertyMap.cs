using System.Reflection;

namespace Marvin.Model
{
    internal class ParameterPropertyMap
    {
        public ParameterPropertyMap(ParameterInfo parameter, PropertyInfo property)
        {
            Parameter = parameter;
            Property = property;
        }

        public ParameterInfo Parameter { get; }

        public PropertyInfo Property { get; }
    }
}