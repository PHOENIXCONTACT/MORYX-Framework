using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_AbortedState), ResourceType = typeof(Strings))]
    internal sealed class AbortedState : OperationDataStateBase
    {
        public AbortedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Aborted)
        {
        }
    }
}