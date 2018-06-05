using System.Runtime.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.DependentTestModule
{
    [DataContract]
    public class SimpleHelloWorldWcfConnectorConfig : BasicWcfConnectorConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHelloWorldWcfConnectorConfig"/> class.
        /// </summary>
        public SimpleHelloWorldWcfConnectorConfig() : base(SimpleHelloWorldWcfService.ServiceName)
        {
            ConnectorHost.BindingType = ServiceBindingType.BasicHttp;
        }

        public override string PluginName { get { return SimpleHelloWorldWcfConnector.PluginName; }}
    }
}