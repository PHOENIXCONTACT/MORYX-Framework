using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.WebServer
{
    /// <summary>
    /// Configuration of the web server.
    /// </summary>
    [DataContract]
    public class WebServerConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// Constructor for web server config. Creates an endpoint with name "MaintenanceWeb".
        /// </summary>
        public WebServerConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                Endpoint = "MaintenanceWeb",
            };
        }

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => WebServerPlugin.PluginName;
    }
}
