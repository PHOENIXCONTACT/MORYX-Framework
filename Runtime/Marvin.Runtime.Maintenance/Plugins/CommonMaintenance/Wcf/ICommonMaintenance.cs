using System;
using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.CommonMaintenance.Wcf
{
    /// <summary>
    /// Service contract for the common maintenance.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.0.0", MinClientVersion = "1.0.0.0")]
    public interface ICommonMaintenance
    {
        /// <summary>
        /// Get the server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        [OperationContract]
        DateTime GetServerTime();
    }
}
