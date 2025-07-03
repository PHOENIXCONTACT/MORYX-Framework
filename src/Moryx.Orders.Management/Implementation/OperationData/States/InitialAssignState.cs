using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InitialAssignState), ResourceType = typeof(Strings))]
    internal sealed class InitialAssignState : OperationDataStateBase
    {
        public override bool IsAssigning => true;

        public InitialAssignState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Initial)
        {
        }

        public override void Assign()
        {
            // Is already creating
        }

        public override void AssignCompleted(bool success)
        {
            Context.HandleAssignCompleted(success);
            NextState(success ? StateReady : StateInitialAssignFailed);
        }

        public override void Abort()
        {
            // cannot be aborted, but should also not throw an error
        }
    }
}