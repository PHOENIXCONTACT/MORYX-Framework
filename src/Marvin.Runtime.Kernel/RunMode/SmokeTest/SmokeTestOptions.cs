using CommandLine;

namespace Marvin.Runtime.Kernel.SmokeTest
{
    internal class SmokeTestOptions
    {
        [Option('e', "expected", Required = true, HelpText = "Expected number of runtime modules.", Default = 3)]
        public int ExpectedModules { get; set; }

        [Option('f', "full", Required = false, HelpText = "If set, full test will be executed")]
        public bool FullTest { get; set; }

        [Option('p', "portIncrement", Required = false, HelpText = "Increments all ports for the SmokeTest", Default = 0)]
        public int PortIncrement { get; set; }

        [Option('i', "interval", Required = false, HelpText = "Time in ms to wait for a state change of a module.", Default = 60000)]
        public int NoChangeInterval { get; set; }
    }
}