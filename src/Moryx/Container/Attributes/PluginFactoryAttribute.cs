// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Interface for plguin factories within the local container
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class PluginFactoryAttribute : FactoryRegistrationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PluginFactoryAttribute()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectorType"></param>
        public PluginFactoryAttribute(Type selectorType) : base(selectorType)
        {

        }
    }
}
