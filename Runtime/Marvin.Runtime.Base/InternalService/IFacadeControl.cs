using System;

namespace Marvin.Runtime.Base
{
    /// <summary>
    /// Interface for all facades used by PluginBase
    /// </summary>
    public interface IFacadeControl
    {
        /// <summary>
        /// Delegate to validate if execution is currently allowed
        /// </summary>
        Action ValidateHealthState { get; set; }
    }
}
