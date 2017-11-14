using System.Linq;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    internal class PausableStep : WorkplanStepBase
    {
        /// 
        public override string Name
        {
            get { return "Pausable"; }
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new PausableTransition();
        }
    }

    internal class PausableTransition : TransitionBase<MainToken>
    {
        ///
        protected override void InputTokenAdded(object sender, IToken token)
        {
            ((IPlace)sender).Remove(token);
            StoredTokens.Add(token);
        }

        /// <summary>
        /// Pause this transitions or finish up quickly
        /// </summary>
        public override void Pause()
        {
            State = (MainToken) StoredTokens.First();
            base.Pause();
        }

        /// <summary>
        /// Resume execution of this transition, if we hold any tokens
        /// </summary>
        public override void Resume()
        {
            PlaceToken(Outputs[0], StoredTokens.First());
            base.Resume();
        }
    }
}