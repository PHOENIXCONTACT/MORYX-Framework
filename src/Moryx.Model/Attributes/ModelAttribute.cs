// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;

namespace Moryx.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute : Attribute
    {
        public string Name { get; }

        public Type ConfiguratorType { get; }

        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        public ModelAttribute(string modelName, Type configuratorType)
        {
            Name = modelName;
            ConfiguratorType = configuratorType;
        }
    }
}
