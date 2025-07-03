using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_ReadyAssignFailedState), ResourceType = typeof(Strings))]
    internal sealed class ReadyAssignFailedState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool IsFailed => true;

        public ReadyAssignFailedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Ready)
        {
        }

        public override void Assign()
        {
            NextState(StateReadyAssign);
            Context.HandleReassign();
        }

        public override void Resume()
        {
            NextState(StateReady);
        }

        public override void Abort()
        {
            NextState(StateAborted);
            Context.HandleAbort();
        }
    }
}