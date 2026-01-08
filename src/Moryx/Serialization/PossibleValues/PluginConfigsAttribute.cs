// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Serialization
{
    /// <summary>
    /// Similar to the <see cref="PluginNameSelectorAttribute"/> this attribute provides a list of component configurations.
    /// These configurations are expected by the implementations of a certain interface and allow each implementation to define its own configuration.
    /// </summary>
    public class PluginConfigsAttribute : PossibleValuesAttribute
    {
        private readonly bool _exportBaseType;

        /// <summary>
        /// Type of the service which should be selectable
        /// </summary>
        protected Type StrategyService { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="PluginConfigsAttribute"/>
        /// </summary>
        /// <param name="strategyService">Type of the service which should be selectable</param>
        public PluginConfigsAttribute(Type strategyService) : this(strategyService, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PluginConfigsAttribute"/>
        /// </summary>
        /// <param name="strategyService">Type of the service which should be selectable</param>
        /// <param name="exportBaseType">If <c>true</c>, the base type will also be exported</param>
        public PluginConfigsAttribute(Type strategyService, bool exportBaseType)
        {
            _exportBaseType = exportBaseType;
            StrategyService = strategyService;
        }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
        {
            var possibleValues = GetPossibleTypes(container);
            return possibleValues.Select(configType => configType.Name);
        }

        /// <inheritdoc />
        public override bool OverridesConversion => true;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => true;

        /// <inheritdoc />
        public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
        {
            var possibleTypes = GetPossibleTypes(container);
            return Activator.CreateInstance(possibleTypes.First(type => type.Name == value));
        }

        private IEnumerable<Type> GetPossibleTypes(IContainer pluginContainer)
        {
            var possibleValues = new List<Type>();
            if (_exportBaseType)
            {
                Type targetApi;
                var configuredInitializable = StrategyService.GetInterface(typeof(IConfiguredInitializable<>).Name);
                if (configuredInitializable != null)
                {
                    targetApi = configuredInitializable;
                }
                else
                {
                    targetApi = StrategyService.GetInterface(typeof(IAsyncConfiguredInitializable<>).Name);
                }

                if (targetApi == null)
                {
                    return [];
                }

                var baseConfig = targetApi.GetGenericArguments()[0];
                if (!baseConfig.IsAbstract)
                    possibleValues.Add(baseConfig);
            }

            if (pluginContainer == null)
                return possibleValues;

            // Find better way to remove ExpectedConfigAttribute -> Tries: 3
            var implementations = pluginContainer.GetRegisteredImplementations(StrategyService);
            possibleValues.AddRange(implementations.Select(implementation => implementation.GetCustomAttribute<ExpectedConfigAttribute>())
                                                   .Where(attribute => attribute != null).Select(attribute => attribute.ExpectedConfigType));
            return possibleValues;
        }
    }
}
