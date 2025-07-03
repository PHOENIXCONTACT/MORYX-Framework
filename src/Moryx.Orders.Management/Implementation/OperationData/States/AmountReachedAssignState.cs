using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_AmountReachedAssignState), ResourceType = typeof(Strings))]
    internal sealed class AmountReachedAssignState : OperationDataStateBase
    {
        public override bool IsAssigning => true;

        public AmountReachedAssignState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Running)
        {
        }

        public override void AssignCompleted(bool success)
        {
            Context.HandleAssignCompleted(success);
            NextState(success ? StateAmountReached : StateAmountReachedAssignFailed);
        }

        public override void Resume()
        {
            NextState(StateAmountReached);
        }
    }
}