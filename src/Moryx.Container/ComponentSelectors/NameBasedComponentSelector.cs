// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Castle.Facilities.TypedFactory;

namespace Moryx.Container
{
    /// <summary>
    /// Component selector for name based selection
    /// </summary>
    internal class NameBasedComponentSelector : DefaultTypedFactoryComponentSelector, INameBasedComponentSelector
    {
        /// <summary>
        /// Selecting the right component name based on first argument
        /// </summary>
        /// <param name="method">Method info</param>
        /// <param name="arguments">Arguments given</param>
        /// <returns>Name of component to select</returns>
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var name = (string)arguments[0];
            return name;
        }
    }
}
