using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Configuration
{
    public class PluginConfigsAttribute : PossibleConfigValuesAttribute
    {
        protected Type StrategyService { get; }

        private readonly bool _exportBaseType;

        public PluginConfigsAttribute(Type strategyService) : this(strategyService, true)
        {
        }

        public PluginConfigsAttribute(Type strategyService, bool exportBaseType)
        {
            _exportBaseType = exportBaseType;
            StrategyService = strategyService;
        }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            var possibleValues = GetPossibleTypes(pluginContainer);
            return possibleValues.Select(configType => configType.Name);
        }

        /// <inheritdoc />
        public override bool OverridesConversion => true;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => true;

        /// <inheritdoc />
        public override object ConvertToConfigValue(IContainer container, string value)
        {
            var possibleTypes = GetPossibleTypes(container);
            return Activator.CreateInstance(possibleTypes.First(type => type.Name == value));
        }

        private IEnumerable<Type> GetPossibleTypes(IContainer pluginContainer)
        {
            var possibleValues = new List<Type>();
            if (_exportBaseType)
            {
                var baseConfig = StrategyService.GetInterface(typeof (IConfiguredModulePlugin<>).Name).GetGenericArguments()[0];
                possibleValues.Add(baseConfig);
            }

            if (pluginContainer == null)
                return possibleValues;

            //TODO: find better way to remove ExpectedConfigAttribute ?!
            var implementations = pluginContainer.GetRegisteredImplementations(StrategyService);
            possibleValues.AddRange(implementations.Select(implementation => implementation.GetCustomAttribute<ExpectedConfigAttribute>())
                                                   .Where(attribute => attribute != null).Select(attribute => attribute.ExcpectedConfigType));
            return possibleValues;
        }
    }
}