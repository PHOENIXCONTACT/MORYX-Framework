using CommandLine;

namespace Marvin.Runtime.Kernel
{
    internal class SystemTestOptions
    {
        [Option('p', "portIncrement", Required = false, HelpText = "Increments all ports for the SystemTest", Default = 0)]
        public int PortIncrement { get; set; }

        [Option('t', "shutdown", Required = false, HelpText = "", Default = 300)]
        public int ShutdownTimeout { get; set; }
    }
}