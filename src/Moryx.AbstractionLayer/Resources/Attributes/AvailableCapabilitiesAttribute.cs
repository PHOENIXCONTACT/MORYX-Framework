// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.Container;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Attribute to configure capabilities in resource configs
    /// </summary>
    public class AvailableCapabilitiesAttribute : PossibleValuesAttribute
    {
        private readonly Func<Type, bool> _capabilitiesFilter;

        /// <inheritdoc />
        public override bool OverridesConversion => true;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;
        
        /// <summary>
        /// List all capabilities
        /// </summary>
        public AvailableCapabilitiesAttribute()
        {
            _capabilitiesFilter = pair => true;
        }

        /// <summary>
        /// Only those capabilities
        /// </summary>
        public AvailableCapabilitiesAttribute(params Type[] types)
        {
            _capabilitiesFilter = types.Contains;
        }

        /// <summary>
        /// Only capabilities based on this type
        /// </summary>
        public AvailableCapabilitiesAttribute(Type baseType)
        {
            _capabilitiesFilter = baseType.IsAssignableFrom;
        }

        /// <summary>
        /// Names of available capabilities
        /// </summary>
        public override IEnumerable<string> GetValues(IContainer pluginContainer)
        {
            return GetCapabilities().Keys;
        }

        /// <summary>
        /// Create capabilities object from name
        /// </summary>
        public override object Parse(IContainer container, string value)
        {
            var capabilitiesType = GetCapabilities()[value];
            return Instanciate(capabilitiesType);
        }

        private static object Instanciate(Type type)
        {
            // Value Types can be instanciated like this
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            // Check if there is a default contructor for that type
            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            if (defaultCtor != null)
                return Activator.CreateInstance(type);

            // If nothing helps: get a parameter constructor and instanciate default values for the parameters.
            var constructorInfo = type.GetConstructors().First();

            object[] ctorArgs = null;
            var parameters = constructorInfo.GetParameters();
            ctorArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                // recursion --> see recursion
                ctorArgs[i] = Instanciate(parameterInfo.ParameterType);
            }
            
            return Activator.CreateInstance(type, ctorArgs);
        }


        private IDictionary<string, Type> GetCapabilities()
        {
            return ReflectionTool.GetPublicClasses<ConcreteCapabilities>(type => _capabilitiesFilter(type))
                    .ToDictionary(t => t.Name, t => t);
        }
    }
}
