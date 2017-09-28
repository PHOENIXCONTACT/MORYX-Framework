using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Modules.Server
{
    /// <summary>
    /// Different health states a server module can have. They are orginized as flags for extensibility
    /// Flags: Running  Error  Changing  Startable
    /// </summary>
    [Flags]
    public enum ServerModuleState
    {
        /// <summary>
        /// Initial value
        /// </summary>
        Stopped = 0x0,
        /// <summary>
        /// Module is initializing
        /// </summary>
        Initializing = 0x2,
        /// <summary>
        /// Service is ready to be started
        /// </summary>
        Ready = 0x1,
        /// <summary>
        /// Service is starting
        /// </summary>
        Starting = 0x3,
        /// <summary>
        /// Service is running
        /// </summary>
        Running = 0x8,
        /// <summary>
        /// Service is stopping
        /// </summary>
        Stopping = 0xA,
        /// <summary>
        /// Service encountered in error during boot or shutdown but the process was completed
        /// </summary>
        BootWarning = 0x6,
        /// <summary>
        /// Service encountered an error during runtime but is still running
        /// </summary>
        Warning = 0xC,
        /// <summary>
        /// Service failed
        /// </summary>
        Failure = 0x4
    }
}
