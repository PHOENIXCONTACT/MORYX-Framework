// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Attributes
{
    /// <summary>
    /// Registration attribute data model setups
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelSetupAttribute : Attribute
    {
        /// <summary>
        /// Target context of the model setup
        /// </summary>
        public Type TargetContext { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ModelSetupAttribute"/>
        /// </summary>
        public ModelSetupAttribute(Type targetContext)
        {
            TargetContext = targetContext;
        }
    }
}