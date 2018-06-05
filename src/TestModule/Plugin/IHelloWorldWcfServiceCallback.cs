using System.ServiceModel;

namespace Marvin.TestModule
{
    public interface IHelloWorldWcfServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void HelloCallback(string message);

        [OperationContract(IsOneWay = false)]
        string ThrowCallback(string message);

    }
}