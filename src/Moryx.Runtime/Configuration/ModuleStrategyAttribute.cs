// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Configuration
{
    /// <summary>
    /// Interface for config attributes to declare the are decorating a module strategy
    /// </summary>
    public class ModuleStrategyAttribute : Attribute
    {
        /// <summary>
        /// Declare this config property as a strategy selector
        /// </summary>
        public ModuleStrategyAttribute(Type strategy)
        {
            Strategy = strategy;
        }

        /// <summary>
        /// Strategy configured with this property
        /// </summary>
        public Type Strategy { get; private set; }
    }
}
