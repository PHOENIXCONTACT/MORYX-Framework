using System;
using System.ServiceModel;
using Marvin.Container;

namespace Marvin.Runtime.Maintenance.Plugins.CommonMaintenance.Wcf
{
    /// <summary>
    /// Wcf service implementations for the common maintenance.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Transient, typeof(ICommonMaintenance))]
    public class CommonMaintenance : ICommonMaintenance
    {
        /// <summary>
        /// Get the current server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }
    }
}
