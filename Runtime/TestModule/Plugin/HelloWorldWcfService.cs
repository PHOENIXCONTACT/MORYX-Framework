using System.ServiceModel;
using Marvin.Container;
using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Singleton, typeof(IHelloWorldWcfService))]
    public class HelloWorldWcfService : SessionService<IHelloWorldWcfServiceCallback, IHelloWorldWcfSvcMgr>, IHelloWorldWcfService
    {
        public const string ServerVersion = "4.7.1.1";
        public const string MinClientVersion = "4.2.0.0";
        public const string ServiceName = "HelloWorldWcfService";

        public string Hello(string name)
        {
            return ServiceManager.Hello(name);
        }

        public string Throw(string name)
        {
            return ServiceManager.Throw(name);
        }

        public void TriggerHelloCallback(string name)
        {
            ServiceManager.TriggerHelloCallback(name);
        }

        public void TriggerThrowCallback(string name)
        {
            ServiceManager.TriggerThrowCallback(name);
        }

        public void HelloCallback(string message)
        {
            Callback.HelloCallback(message);
        }

        public string ThrowCallback(string message)
        {
            return Callback.ThrowCallback(message);
        }
    }
}