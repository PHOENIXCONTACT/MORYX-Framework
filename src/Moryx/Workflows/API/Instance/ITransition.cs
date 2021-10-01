// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Workflows
{
    /// <summary>
    /// Interface representing a transition
    /// </summary>
    public interface ITransition : ITokenHolder
    {
        /// <summary>
        /// Flag if this transitions is currently executing code. This must only be used by fast triggering
        /// transitions or transitions that complete their current execution. It must not be used to await external input.
        /// </summary>
        bool Executing { get; }

        /// <summary>
        /// Wire the transition
        /// </summary>
        void Initialize();

        /// <summary>
        /// All inputs of this transition, the amount depends on the workflow
        /// </summary>
        IPlace[] Inputs { get; }

        /// <summary>
        /// All outputs of this transition, the amount depends on the type of transition
        /// </summary>
        IPlace[] Outputs { get; }
    }

    /// <summary>
    /// Public transition relevant outside of the engine
    /// </summary>
    public interface IObservableTransition : ITransition
    {
        /// <summary>
        /// Event raised, when the transition was triggered
        /// </summary>
        event EventHandler Triggered;
    }
}
