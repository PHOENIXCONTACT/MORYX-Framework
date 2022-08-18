// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Workflows;
using Moryx.Workflows.Transitions;
using Moryx.Workflows.WorkplanSteps;

namespace Moryx.Tests.Workflows
{
    internal class PausableStep : WorkplanStepBase
    {
        public PausableStep()
        {
            Name = "Pausable";
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
