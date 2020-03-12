// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Configuration;

namespace Marvin.Runtime.Kernel.Tests.Dummys
{
    /// <summary>
    /// Test subconfig
    /// </summary>
    public class RuntimeConfigManagerTestConfig2 : IConfig
    {
        /// <summary>
        /// Current state of the config object.
        /// </summary>
        public ConfigState ConfigState
        {
            get; set; 
        }

        /// <summary>
        /// Exception message if load failed.
        /// </summary>
        public string LoadError
        {
            get; set;
        }
    }
}
