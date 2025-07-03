using System.Threading.Tasks;
using Moryx.Modules;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Represents a step for the assignment of a specific operation
    /// </summary>
    internal interface IOperationAssignStep : IPlugin
    {
        /// <summary>
        /// Execute step for the assignment
        /// </summary>
        Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger);

        /// <summary>
        /// Executes the restore
        /// </summary>
        Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger);
    }
}