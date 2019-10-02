using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Base contract for all RunModes
    /// </summary>
    public interface IRunMode
    {
        /// <summary>
        /// Service manager instance
        /// </summary>
        IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Setup the environment by passing the command line arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        void Setup(RuntimeArguments args);

        /// <summary>
        /// Run environment
        /// </summary>
        /// <returns>0: All fine - 1: Warning - 2: Error</returns>
        RuntimeErrorCode Run();
    }
}
