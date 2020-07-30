// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelConfiguratorAttribute : Attribute
    {
        public Type ConfiguratorType { get; }

        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        public ModelConfiguratorAttribute(Type configuratorType)
        {
            ConfiguratorType = configuratorType;
        }
    }
}
