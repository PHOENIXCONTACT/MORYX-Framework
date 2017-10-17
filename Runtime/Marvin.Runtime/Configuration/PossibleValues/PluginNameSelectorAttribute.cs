using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;

namespace Marvin.Runtime.Configuration
{
    public class PluginNameSelectorAttribute : PossibleConfigValuesAttribute
    {
        private readonly Type _componentType;
        public PluginNameSelectorAttribute(Type componentType)
        {
            _componentType = componentType;
        }

        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            return (pluginContainer == null ? AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                                                                     .Where(type => _componentType.IsAssignableFrom(type) && _componentType != type)
                                            : pluginContainer.GetRegisteredImplementations(_componentType))
                   .Select(GetComponentName);
        }

        public override bool OverridesConversion
        {
            get { return false; }
        }

        public override bool UpdateFromPredecessor
        {
            get { return false; }
        }

        private string GetComponentName(Type component)
        {
            var att = component.GetCustomAttribute<RegistrationAttribute>();
            return (att == null || string.IsNullOrEmpty(att.Name)) ? component.FullName : att.Name;
        }
    }
}
