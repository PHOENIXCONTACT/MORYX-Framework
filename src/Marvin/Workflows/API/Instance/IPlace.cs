// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Workflows
{
    /// <summary>
    /// Interface for a place
    /// </summary>
    public interface IPlace : ITokenHolder
    {
        /// <summary>
        /// Classification of this place
        /// </summary>
        NodeClassification Classification { get; }

        /// <summary>
        /// Add token to place
        /// </summary>
        void Add(IToken token);

        /// <summary>
        /// Remove token from 
        /// </summary>
        void Remove(IToken token);

        /// <summary>
        /// Event raised when token was added to input
        /// </summary>
        event EventHandler<IToken> TokenAdded;

        /// <summary>
        /// Event raised, when a token was removed
        /// </summary>
        event EventHandler<IToken> TokenRemoved;
    }
}
