using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Diagnostics.ClientLog.Wcf
{
    /// <summary>
    /// Service contract to log.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(MinClientVersion = "1.0.0.0", ServerVersion = "1.0.0.0")]
    public interface IRemoteLogging
    {
        /// <summary>
        /// Log.
        /// </summary>
        [OperationContract]
        void Log();
    }
}