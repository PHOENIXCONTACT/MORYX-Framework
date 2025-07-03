using Moryx.Container;

namespace Moryx.Orders.Management.Assignment
{
    [PluginFactory(typeof(INameBasedComponentSelector))]
    internal interface IAssignStepFactory
    {
        IOperationAssignStep Create(string name);
    }
}