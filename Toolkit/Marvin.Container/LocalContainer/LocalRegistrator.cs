using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Marvin.Container
{
    internal class LocalRegistrator : ComponentRegistrator
    {
        private readonly IDictionary<Type, string> _strategies;

        public LocalRegistrator(IDictionary<Type, string> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Method to determine if this component shall be installed
        /// </summary>
        public override bool ShallInstall(Type foundType)
        {
            var regAtt = foundType.GetCustomAttribute<ComponentAttribute>();
            var facAtt = foundType.GetCustomAttribute<PluginFactoryAttribute>();

            return (regAtt != null || facAtt != null) && NotRegisteredYet(foundType, regAtt);
        }

        /// <summary>
        /// Determine a possible override for this member. Base implementatin checks for named attribute
        /// </summary>
        protected override ServiceOverride OverrideDependency(string dependencyName, Type dependencyType, ICustomAttributeProvider attributeProvider)
        {
            var dependency = base.OverrideDependency(dependencyName, dependencyType, attributeProvider);

            if (dependency == null && _strategies.ContainsKey(dependencyType))
                dependency = Dependency.OnComponent(dependencyName, _strategies[dependencyType]);

            return dependency;
        }
    }
}
