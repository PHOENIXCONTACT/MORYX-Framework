// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines;

/// <summary>
/// Base class for synchronous state machine states
/// </summary>
public abstract class SyncStateBase : StateBase
{
    /// <summary>
    /// Context of the state machine
    /// </summary>
    protected new IStateContext Context => (IStateContext)base.Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateBase"/> class.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <param name="stateMap">Map of states to objects</param>
    protected SyncStateBase(IStateContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    /// <summary>
    /// Will be called while entering the next state
    /// </summary>
    public virtual void OnEnter()
    {

    }

    /// <summary>
    /// Will be called while exiting the current state
    /// </summary>
    public virtual void OnExit()
    {

    }

    /// <summary>
    /// Create a state machine of the given base type and will set it on the given context
    /// Will internally called by the <see cref="StateMachine"/> wrapper class
    /// </summary>
    internal static void Create(Type stateBaseType, IStateContext context, int? initialKey)
    {
        if (!typeof(SyncStateBase).IsAssignableFrom(stateBaseType))
            throw new InvalidOperationException($"Only states inherited from {nameof(SyncStateBase)} are supported!");

        var initialState = CreateMapAndGetInitial(stateBaseType, context, initialKey) as SyncStateBase;
        if (initialState == null)
            throw new ArgumentException($"Initial state does not inherit from {nameof(SyncStateBase)}");

        context.SetState(initialState);
        initialState.OnEnter();
    }

    /// <summary>
    /// Forces a specific state with option to exit the current and enter the forced state
    /// </summary>
    internal virtual void Force(int state, bool exitCurrent, bool enterForced)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException("State cannot be forced. " +
                                                $"The state {state} does not exist in StateMachine.");
        }

        var nextState = (SyncStateBase)next;

        // If requested, exit current state
        if (exitCurrent)
            OnExit();

        // Set next state
        Context.SetState(nextState);

        // If requested, enter forced state
        if (enterForced)
            nextState.OnEnter();
    }

    /// <summary>
    /// Jump to next state
    /// </summary>
    /// <param name="state">Number of the next state</param>
    protected virtual void NextState(int state)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException($"The state {state} does not exist in StateMachine.");
        }

        var nextState = (SyncStateBase)next;

        // Exit the old state
        OnExit();

        // Set next state
        Context.SetState(nextState);

        // Enter the next state
        nextState.OnEnter();
    }
}

/// <summary>
/// Base class for synchronous state machine states
/// </summary>
/// <typeparam name="TContext">Typed context</typeparam>
public abstract class SyncStateBase<TContext> : SyncStateBase
    where TContext : IStateContext
{
    /// <summary>
    /// Context of the state machine
    /// </summary>
    protected new TContext Context => (TContext)base.Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateBase"/> class.
    /// </summary>
    /// <param name="stateMap">Map of states to objects</param>
    /// <param name="context">Context of the state machine</param>
    protected SyncStateBase(TContext context, StateMap stateMap) : base(context, stateMap)
    {
    }
}
