using Marvin.Runtime.Configuration;
using Marvin.Runtime.HeartOfGold.Runmodes;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Environment wrapper for the windows service
    /// </summary>
    [Runmode(RunModeName)]
    public class ServiceRunMode : IRunmode
    {
        /// <summary>
        /// Const name of the RunMode. 
        /// </summary>
        public const string RunModeName = "WinService";

        /// <summary>
        /// Service manager instance
        /// </summary>
        public IModuleManager ModuleManager { get; set; }
        /// <summary>
        /// Config manager instance.
        /// </summary>
        public IRuntimeConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Setup the environment by passing the command line arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public void Setup(RuntimeArguments args)
        {
        }

        /// <summary>
        /// Run environment
        /// </summary>
        /// <returns>0: All fine - 1: Warning - 2: Error</returns>
        public RunModeErrorCode Run()
        {
            var service = new MarvinService(ModuleManager, ConfigManager);
            service.Run();
            return RunModeErrorCode.NoError;
        }
    }
}