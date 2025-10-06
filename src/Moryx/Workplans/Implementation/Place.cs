// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Implementation of the IPlace interface with with events
    /// </summary>
    public class Place : IPlace
    {
        private bool _running = true;

        #region ITokenHolder

        /// 
        public long Id { get; set; }

        private IList<IToken> _tokens = new List<IToken>();
        /// 
        public IEnumerable<IToken> Tokens
        {
            get { return _tokens; }
            set { _tokens = new List<IToken>(value); }
        }

        /// <summary>
        /// Internal state of the place is irrelevant for default places 
        /// </summary>
        public virtual object InternalState { get; set; }

        ///
        public void Pause()
        {
            _running = false;
        }

        /// 
        public void Resume()
        {
            // Copy tokens because they might be moved the moment we start
            foreach (var token in Tokens.ToArray())
            {
                TokenAdded(this, token);
            }
            _running = true;
        }

        #endregion

        #region IPlace

        /// <summary>
        /// Classification of this place
        /// </summary>
        public NodeClassification Classification { get; set; }

        /// <summary>
        /// Add a token to this place. As long as execution is running the place will raise the 
        /// <see cref="TokenAdded"/> event.
        /// </summary>
        public virtual void Add(IToken token)
        {
            _tokens.Add(token);
            if (_running)
                TokenAdded(this, token);
        }

        /// <summary>
        /// Will remove the token as long as the execution is running. Otherwise it will keep
        /// the token for the snapshot
        /// </summary>
        public virtual void Remove(IToken token)
        {
            if (_tokens.Remove(token))
                TokenRemoved?.Invoke(this, token);
        }

        /// 
        public event EventHandler<IToken> TokenAdded;

        /// 
        public event EventHandler<IToken> TokenRemoved;

        #endregion

    }
}
