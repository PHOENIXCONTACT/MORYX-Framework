// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Option class for the <see cref="SystemTest"/>
    /// </summary>
    [Verb("systemTest", HelpText = "Starts the runtime in system test mode. Less console output and more options.")]
    public class SystemTestOptions : RuntimeOptions
    {
        /// <summary>
        /// Increments all ports for the SystemTest
        /// </summary>
        [Option('p', "portIncrement", Required = false, Default = 0, HelpText = "Increments all ports for the SystemTest.")]
        public int PortIncrement { get; set; }

        /// <summary>
        /// Timeout to wait for a module shutdown
        /// </summary>
        [Option('t', "shutdown", Required = false, Default = 300, HelpText = "Timeout to wait for a module shutdown.")]
        public int ShutdownTimeout { get; set; }

        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Custom config directory", new SystemTestOptions { ConfigDir = @"C:\YourApp\Config"}),
                new Example("Add port increment for all http and net.tcp ports", new SystemTestOptions { PortIncrement = 4711}),
                new Example("Override shutdown time out", new SystemTestOptions { ShutdownTimeout = 800}),
            };
    }
}
