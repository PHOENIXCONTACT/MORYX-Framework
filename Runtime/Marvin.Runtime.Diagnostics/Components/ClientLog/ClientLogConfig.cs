using System.Runtime.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Diagnostics.ClientLog
{
    /// <summary>
    /// Configuration for the client log.
    /// </summary>
    [DataContract]
    public class ClientLogConfig : DiagnosticsPluginConfigBase
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public override string PluginName { get { return ClientLog.PluginName; } }

        /// <summary>
        /// Configuration of the remote log host.
        /// </summary>
        [DataMember]
        public HostConfig RemoteLogHost { get; set; }
    }
}