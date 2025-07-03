using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_CompletedState), ResourceType = typeof(Strings))]
    internal sealed class CompletedState : OperationDataStateBase
    {
        public CompletedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Completed)
        {
        }

        public override bool CanAdvice => true;

        public override void Resume()
        {
        }

        public override void Advice(OperationAdvice advice)
        {
            Context.HandleAdvice(advice);
        }
    }
}
