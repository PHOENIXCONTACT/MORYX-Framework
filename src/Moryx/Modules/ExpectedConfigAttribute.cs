// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Modules
{
    /// <summary>
    /// Attribute to decorate a <see cref="IPlugin"/> to receive a certain config type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpectedConfigAttribute : Attribute
    {
        /// <summary>
        /// Config type expected by this <see cref="IPlugin"/>
        /// </summary>
        public Type ExpectedConfigType { get; }

        /// <summary>
        /// State that this <see cref="IPlugin"/> requires config instances of the given type
        /// </summary>
        public ExpectedConfigAttribute(Type configType)
        {
            ExpectedConfigType = configType;
        }
    }
}
