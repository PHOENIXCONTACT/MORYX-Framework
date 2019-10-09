using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Marvin.Runtime.Kernel.SmokeTest
{
    /// <summary>
    /// Starts the runtime and executes the smoke tests.
    /// </summary>
    [Verb("smokeTest", HelpText = "Starts the runtime and executes the smoke tests.")]
    public class SmokeTestOptions : RuntimeOptions
    {
        /// <summary>
        /// Expected number of runtime modules.
        /// </summary>
        [Option('e', "expected", Required = true, HelpText = "Expected number of runtime modules.", Default = 3)]
        public int ExpectedModules { get; set; }

        /// <summary>
        /// If set, full test will be executed
        /// </summary>
        [Option('f', "full", Required = false, HelpText = "If set, full test will be executed.")]
        public bool FullTest { get; set; }

        /// <summary>
        /// Increments all ports for the SmokeTest
        /// </summary>
        [Option('p', "portIncrement", Required = false, HelpText = "Increments all ports for the SmokeTest.", Default = 0)]
        public int PortIncrement { get; set; }

        /// <summary>
        /// Time in ms to wait for a state change of a module.
        /// </summary>
        [Option('i', "interval", Required = false, HelpText = "Time in ms to wait for a state change of a module.", Default = 60000)]
        public int NoChangeInterval { get; set; }

        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Sample with 5 expected modules", new SmokeTestOptions { ExpectedModules = 5, PortIncrement = 4711})
            };
    }
}