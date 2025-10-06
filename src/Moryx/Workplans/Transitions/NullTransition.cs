// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Transitions
{
    /// <summary>
    /// Special transition that can be used to create optional steps in workplans. If a condition for
    /// a <see cref="IWorkplanStep"/> is not fulfilled it can create a <see cref="NullTransition"/>
    /// instead of the real <see cref="ITransition"/>.
    /// </summary>
    public class NullTransition : TransitionBase
    {
        private readonly int _index;

        /// <summary>
        /// Create <see cref="NullTransition"/> that places the token always
        /// on the first output, assuming it means success.
        /// </summary>
        public NullTransition()
        {
        }

        /// <summary>
        /// Create <see cref="NullTransition"/> and define the index of the output to place the token
        /// </summary>
        public NullTransition(int index)
        {
            _index = index;
        }

        ///
        protected override void InputTokenAdded(object sender, IToken token)
        {
            Executing(delegate
            {
                TakeToken((IPlace)sender, token);
                PlaceToken(Outputs[_index], token);
            });
        }
    }
}
