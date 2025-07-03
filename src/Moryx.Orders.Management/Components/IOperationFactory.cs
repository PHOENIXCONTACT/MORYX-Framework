using Moryx.Container;

namespace Moryx.Orders.Management
{
    [PluginFactory]
    internal interface IOperationFactory
    {
        IOperationData Create();

        void Destroy(IOperationData operationData);
    }
}