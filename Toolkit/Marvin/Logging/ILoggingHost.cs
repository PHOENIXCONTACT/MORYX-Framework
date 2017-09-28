using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Logging
{
    /// <summary>
    /// Interface for all components hosting a top level logger
    /// </summary>
    public interface ILoggingHost
    {
        /// <summary>
        /// Name of this host. Used for logger name structure
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Logger instance
        /// </summary>
        IModuleLogger Logger { get; set; }
    }
}
