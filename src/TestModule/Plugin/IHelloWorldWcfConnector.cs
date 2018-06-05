using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    public interface IHelloWorldWcfConnector : IWcfConnector<HelloWorldWcfConnectorConfig>
    {
        void TriggerHelloCallback(string name);
    }
}