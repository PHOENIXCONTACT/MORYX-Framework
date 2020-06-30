// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Option class for the <see cref="DeveloperConsole"/>
    /// </summary>
    [Verb(VerbName, HelpText = "Starts the runtime with the developer console.")]
    public class DeveloperConsoleOptions : RuntimeOptions
    {
        /// <summary>
        /// Name of the verb
        /// </summary>
        internal const string VerbName = "dev";

        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Starts developer console with custom config directory", new DeveloperConsoleOptions { ConfigDir = @"C:\YourApp\Config"})
            };
    }
}
