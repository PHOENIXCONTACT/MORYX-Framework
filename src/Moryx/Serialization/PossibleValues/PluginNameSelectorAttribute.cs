// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;

namespace Moryx.Serialization
{
    /// <summary>
    /// <see cref="PossibleValuesAttribute"/> to provide possible plugin names
    /// </summary>
    public class PluginNameSelectorAttribute : PossibleValuesAttribute
    {
        private readonly Type _componentType;

        /// <summary>
        /// Creates a new instance of <see cref="PossibleValuesAttribute"/>
        /// </summary>
        public PluginNameSelectorAttribute(Type componentType)
        {
            _componentType = componentType;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
        {
            return (container == null
                    ? AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                        .Where(type => _componentType.IsAssignableFrom(type) && _componentType != type)
                    : container.GetRegisteredImplementations(_componentType))
                .Select(GetComponentName);
        }

        private static string GetComponentName(Type component)
        {
            var att = component.GetCustomAttribute<ComponentAttribute>();
            return string.IsNullOrEmpty(att?.Name) ? component.FullName : att.Name;
        }

        /// <inheritdoc />
        public override bool OverridesConversion => false;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;
    }
}
