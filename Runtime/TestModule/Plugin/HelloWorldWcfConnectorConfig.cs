using System.Runtime.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    [DataContract]
    public class HelloWorldWcfConnectorConfig : BasicWcfConnectorConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldWcfConnectorConfig"/> class.
        /// </summary>
        public HelloWorldWcfConnectorConfig() : base(HelloWorldWcfService.ServiceName)
        {
        }

        public override string PluginName { get { return HelloWorldWcfConnector.PluginName; }}
    }
}