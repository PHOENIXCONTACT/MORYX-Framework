// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;

namespace Marvin.Workflows.Transitions
{
    /// <summary>
    /// Transition used to join two or more parallel paths
    /// </summary>
    internal sealed class JoinTransition : TransitionBase
    {
        private IToken[] _receivedTokens;

        ///
        protected override void InputTokenAdded(object sender, IToken token)
        {
            lock (this)
                Executing(() => EvaluateTokens(sender, token));
        }

        /// <summary>
        /// Check if we received a token on each input place
        /// </summary>
        private void EvaluateTokens(object sender, IToken token)
        {
            var length = Inputs.Length;
            if (_receivedTokens == null)
            {
                // Create an array with the same size the Inputs array
                _receivedTokens = new IToken[length];
            }

            // Store partial token and check all other tokens simultaniously
            var allSet = true;
            IToken original = null;
            for (var i = 0; i < length; i++)
            {
                if (Inputs[i].Equals(sender))
                    _receivedTokens[i] = token;

                var splitToken = _receivedTokens[i] as SplitToken;
                if (splitToken == null)
                {
                    allSet = false;
                }
                else
                {
                    // Set orginal on first match otherwise skip
                    original = original ?? splitToken.Original;
                }
            }

            if (allSet)
                TriggerTransition(original);
        }

        /// <summary>
        /// Trigger transition and remove all input tokens
        /// </summary>
        private void TriggerTransition(IToken original)
        {
            // Remove all input tokens if output was set
            for (var i = 0; i < Inputs.Length; i++)
            {
                Inputs[i].Remove(_receivedTokens[i]);
            }

            Outputs[0].Add(original);
        }
    }
}
