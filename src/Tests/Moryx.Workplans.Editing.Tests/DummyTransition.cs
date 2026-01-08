// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;

namespace Moryx.Tests.Workplans
{
    public class DummyTransition : TransitionBase, IObservableTransition
    {
        /// <summary>
        /// Name of the DummyTransition
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Context of the DummyTransition
        /// </summary>
        public IWorkplanContext? Context { get; set; }

        public int ResultOutput { get; set; }

        protected override void InputTokenAdded(object sender, IToken token)
        {
            ((IPlace)sender).Remove(token);
            StoredTokens.Add(token);
            Triggered?.Invoke(this, new EventArgs());
            if (ResultOutput >= 0) // Resume directly
                PlaceToken(Outputs[ResultOutput], StoredTokens.First());
        }

        public override void Resume()
        {
            if (StoredTokens.Any())
                Triggered?.Invoke(this, new EventArgs());
        }

        public void ResumeAsync(int result)
        {
            PlaceToken(Outputs[result], StoredTokens.First());
        }

        public event EventHandler? Triggered;
    }
}