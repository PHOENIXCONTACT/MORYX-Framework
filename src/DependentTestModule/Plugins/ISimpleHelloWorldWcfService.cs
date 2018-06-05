using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.DependentTestModule
{
    [ServiceContract]
    [ServiceVersion(ServerVersion = SimpleHelloWorldWcfService.ServerVersion, MinClientVersion = SimpleHelloWorldWcfService.MinClientVersion)]
    public interface ISimpleHelloWorldWcfService
    {
        [OperationContract(IsOneWay = false)]
        string Hello(string name);

        [OperationContract(IsOneWay = false)]
        string Throw(string name);
    }
}