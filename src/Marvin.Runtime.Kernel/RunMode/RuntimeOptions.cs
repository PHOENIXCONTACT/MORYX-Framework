// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using CommandLine;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Base class for run mode options
    /// </summary>
    public abstract class RuntimeOptions
    {
        /// <summary>
        /// Directory where configs are saved.
        /// </summary>
        [Option('c', "configDir", Required = false, Default = "Config", HelpText = "Directory where configs are saved.")]
        public string ConfigDir { get; set; }
    }
}
