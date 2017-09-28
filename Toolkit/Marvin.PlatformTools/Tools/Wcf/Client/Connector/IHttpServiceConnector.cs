using System;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Interface for http service connectors
    /// </summary>
    public interface IHttpServiceConnector : IModulePlugin
    {
        /// <summary>
        /// Informs that the availabilty of the wcf client has changed
        /// </summary>
        event EventHandler AvailabilityChanged;

        /// <summary>
        /// Propertie the check the availability of the wcf client
        /// </summary>
        bool IsAvailable { get; }
    }
}