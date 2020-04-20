// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model
{
    /// <summary>
    /// Attribute for IModelSetups to determine their target model
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelSetupAttribute : Attribute
    {
        public string TargetModel { get; }

        /// <summary>
        /// Constructor used if the <see cref="IModelSetup"/> is only available for the defined model
        /// </summary>
        public ModelSetupAttribute(string targetModel)
        {
            TargetModel = targetModel;
        }
    }
}
