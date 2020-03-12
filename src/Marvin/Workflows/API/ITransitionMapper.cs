// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Workflows
{
    /// <summary>
    /// Interface of the helper component that can be used to create
    /// transition listeners
    /// </summary>
    public interface ITransitionMapper : IDisposable
    {
        /// <summary>
        /// Event handler for <see cref="IWorkflowEngine.TransitionTriggered"/>
        /// </summary>
        void TransitionTriggered(object sender, ITransition transition);

        /// <summary>
        /// Register another <see cref="IAttemptInvokation"/> strategy
        /// </summary>
        ITransitionMapper Map(IAttemptInvokation invokation);

        /// <summary>
        /// Register a typed delegate for a certain 
        /// </summary>
        ITransitionMapper Map<T>(Action<T> transitionHandler)
            where T : class, ITransition;
    }
}
