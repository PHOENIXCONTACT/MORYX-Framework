// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Reflection;

namespace Moryx.Products.Management.NullStrategies
{
    /// <summary>
    /// Strategy configuration for types without specialized properties
    /// </summary>
    public class PropertylessStrategyConfigurationAttribute : StrategyConfigurationAttribute
    {
        public PropertylessStrategyConfigurationAttribute(params Type[] baseTypes) : base(baseTypes)
        {
        }

        /// <summary>
        /// Type compliance is perfect for types without properties, other bad
        /// </summary>
        public override int TypeCompliance(Type targetType)
        {
            // Determine base properties from supported types
            var baseProperties = SupportedTypes
                .SelectMany(t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .Select(prop => prop.Name).Distinct().ToList();
            // Get properties from target type and make sure all are defined by the base class
            var targetProps = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(prop => prop.Name).Distinct();

            return targetProps.All(baseProperties.Contains) ? PerfectMatch : BadCompliance;
        }
    }
}
