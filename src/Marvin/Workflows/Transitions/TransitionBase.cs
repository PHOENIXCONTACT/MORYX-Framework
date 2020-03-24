// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Workflows.Transitions
{
    /// <summary>
    /// Base class for all transition implementations
    /// </summary>
    public abstract class TransitionBase : ITransition
    {
        /// <summary>
        /// Instantiate the transition and prepare the list of stored tokens
        /// </summary>
        protected TransitionBase()
        {
            StoredTokens = new List<IToken>();
        }

        #region ITokenHolder

        /// 
        public long Id { get; set; }

        /// <summary>
        /// All tokens that were taken by this transition and are waiting to be placed on an output
        /// </summary>
        protected IList<IToken> StoredTokens { get; private set; }
        /// 
        public IEnumerable<IToken> Tokens
        {
            get { return StoredTokens; }
            set { StoredTokens = new List<IToken>(value); }
        }

        private bool _executing;
        /// 
        bool ITransition.Executing { get { return _executing; } }

        /// <summary>
        /// Execute operation and set the <see cref="ITransition.Executing"/> flag
        /// </summary>
        protected void Executing(Action action)
        {
            _executing = true;
            try
            {
                action();
            }
            finally
            {
                _executing = false;
            }
        }

        /// <summary>
        /// Internal state of the transition. This should be used to store all information required to restore
        /// the transition in its current state
        /// </summary>
        public virtual object InternalState { get; set; }

        /// <summary>
        /// Pause this transitions or finish up quickly
        /// </summary>
        public virtual void Pause()
        {
        }

        /// <summary>
        /// Take token from place and store in list
        /// </summary>
        protected void TakeToken(IPlace sender, IToken token)
        {
            sender.Remove(token);
            StoredTokens.Add(token);
        }

        /// <summary>
        /// Place a stored token on an output and remove it from <see cref="StoredTokens"/>
        /// </summary>
        /// <returns>True if token was placed otherwise false</returns>
        protected void PlaceToken(IPlace output, IToken token)
        {
            output.Add(token);
            StoredTokens.Remove(token);
        }


        /// <summary>
        /// Resume execution of this transition, if we hold any tokens
        /// </summary>
        public virtual void Resume()
        {
        }

        #endregion

        #region ITransition

        /// 
        public virtual void Initialize()
        {
            foreach (var input in Inputs)
            {
                input.TokenAdded += InputTokenAdded;
            }
        }

        ///
        protected abstract void InputTokenAdded(object sender, IToken token);

        /// 
        public IPlace[] Inputs { get; set; }

        /// 
        public IPlace[] Outputs { get; set; }

        #endregion

        /// <summary>
        /// Create <see cref="IIndexResolver"/> to resolve the output index for a mapping value
        /// </summary>
        public static IIndexResolver CreateIndexResolver(OutputDescription[] descriptions)
        {
            var allEqual = true;
            for (int i = 0; i < descriptions.Length; i++)
            {
                allEqual &= (i == descriptions[i].MappingValue);
            }
            return allEqual ? (IIndexResolver) new CastResolver() : new DescriptionResolver(descriptions);
        }

        /// <summary>
        /// Directly castes the mapping value to the index
        /// </summary>
        private struct CastResolver : IIndexResolver
        {
            /// <summary>
            /// Resolve index by mapping value
            /// </summary>
            public int Resolve(long mappingValue)
            {
                return (int)mappingValue;
            }
        }

        /// <summary>
        /// Resolves index by comparing the mapping value to the OutputDescriptions
        /// </summary>
        private struct DescriptionResolver : IIndexResolver
        {
            private readonly OutputDescription[] _descriptions;

            public DescriptionResolver(OutputDescription[] descriptions)
            {
                _descriptions = descriptions;
            }

            /// <summary>
            /// Resolve index by mapping value
            /// </summary>
            public int Resolve(long mappingValue)
            {
                for (var i = 0; i < _descriptions.Length; i++)
                {
                    if (_descriptions[i].MappingValue == mappingValue)
                        return i;
                }
                throw new ArgumentOutOfRangeException("mappingValue");
            }
        }
    }

    /// <summary>
    /// Transition base that provides typed state object
    /// </summary>
    /// <typeparam name="TInternalState">Type of the state object</typeparam>
    public abstract class TransitionBase<TInternalState> : TransitionBase
        where TInternalState : class, new()
    {
        /// <summary>
        /// Typed state object of the transitions.
        /// </summary>
        protected TInternalState State { get; set; }

        /// <summary>
        /// Internal state of the transition. This should be used to store all information required to restore
        /// the transition in its current state
        /// </summary>
        public override object InternalState
        {
            get { return State; }
            set { State = (TInternalState)value; }
        }
    }
}
