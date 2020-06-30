// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.StateMachines
{
    /// <summary>
    /// Attribute used for identifing the key of a state
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StateDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="StateDefinitionAttribute"/>
        /// </summary>
        /// <param name="type">Type of the responsible state</param>
        public StateDefinitionAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type of the state
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Defines weather the state is the initial state of the machine or not.
        /// </summary>
        public bool IsInitial { get; set; }
    }
}
