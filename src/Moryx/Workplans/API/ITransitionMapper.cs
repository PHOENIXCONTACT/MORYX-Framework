// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

/// <summary>
/// Interface of the helper component that can be used to create
/// transition listeners
/// </summary>
public interface ITransitionMapper : IDisposable
{
    /// <summary>
    /// Event handler for <see cref="IWorkplanEngine.TransitionTriggered"/>
    /// </summary>
    void TransitionTriggered(object sender, ITransition transition);

    /// <summary>
    /// Register another <see cref="IAttemptInvocation"/> strategy
    /// </summary>
    ITransitionMapper Map(IAttemptInvocation invocation);

    /// <summary>
    /// Register a typed delegate for a certain
    /// </summary>
    ITransitionMapper Map<T>(Action<T> transitionHandler)
        where T : class, ITransition;
}