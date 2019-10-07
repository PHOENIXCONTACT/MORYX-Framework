using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Option class for the <see cref="ServiceRunMode"/>
    /// </summary>
    [Verb("service", HelpText = "Starts the runtime in service mode. Used for Windows Services.")]
    public class ServiceOptions : RuntimeOptions
    {
        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Starts in service mode with a custom config directory", new ServiceOptions { ConfigDir = @"C:\YourApp\Config"}),
            };
    }

    /// <summary>
    /// Environment wrapper for the windows service
    /// </summary>
    [RunMode(nameof(ServiceRunMode), typeof(ServiceOptions))]
    public class ServiceRunMode : RunModeBase<ServiceOptions>
    {
        /// <summary>
        /// Config manager instance.
        /// </summary>
        public IRuntimeConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Run environment
        /// </summary>
        /// <returns>0: All fine - 1: Warning - 2: Error</returns>
        public override RuntimeErrorCode Run()
        {
            var service = new MarvinService(ModuleManager, ConfigManager);
            service.Run();
            return RuntimeErrorCode.NoError;
        }
    }
}