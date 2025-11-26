// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines;

public abstract class AsyncStateBase : StateBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateBase"/> class.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <param name="stateMap">Map of states to objects</param>
    protected AsyncStateBase(IStateContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    /// <summary>
    /// Will be called while entering the next state
    /// </summary>
    public virtual Task OnEnterAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Will be called while exiting the current state
    /// </summary>
    public virtual Task OnExitAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override void NextState(int state)
    {
        throw new InvalidOperationException("Synchronous calls are not allowed on AsyncState");
    }

    /// <summary>
    /// Jump to next state async
    /// </summary>
    /// <param name="state">Number of the next state</param>
    protected virtual async Task NextStateAsync(int state)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException($"The state {state} does not exist in StateMachine.");
        }

        // Exit the old state
        await OnExitAsync();

        // Set next state
        Context.SetState(next);

        // Enter the next state
        await ((AsyncStateBase)next).OnEnterAsync();
    }

    /// <inheritdoc />
    internal override void Force(int state, bool exitCurrent, bool enterForced)
    {
        throw new InvalidOperationException("Synchronous calls are not allowed on AsyncState");
    }

    /// <summary>
    /// Forces a specific state with option to exit the current and enter the forced state
    /// </summary>
    internal virtual async Task ForceAsync(int state, bool exitCurrent, bool enterForced)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException("State cannot be forced. " +
                                                $"The state {state} does not exist in StateMachine.");
        }

        // If requested, exit current state
        if (exitCurrent)
            await OnExitAsync();

        // Set next state
        Context.SetState(next);

        // If requested, enter forced state
        if (enterForced)
            await ((AsyncStateBase)next).OnEnterAsync();
    }

    /// <summary>
    /// Create a state machine of the given base type and will set it on the given context
    /// Will internally called by the <see cref="StateMachine"/> wrapper class
    /// </summary>
    internal static Task CreateAsync(Type stateBaseType, IStateContext context, int? initialKey)
    {
        var initialState = (AsyncStateBase)CreateMap(stateBaseType, context, initialKey);

        if (!typeof(AsyncStateBase).IsAssignableFrom(stateBaseType))
        {
            throw new InvalidOperationException("Creating state on async StateBase " +
                                                $"is not supported, use {nameof(StateBase)}.{nameof(StateBase.Create)} instead");
        }

        context.SetState(initialState);
        return initialState.OnEnterAsync();
    }
}

/// <summary>
/// Base class for state machine states
/// </summary>
/// <typeparam name="TContext"></typeparam>
public abstract class AsyncStateBase<TContext> : AsyncStateBase
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
    protected AsyncStateBase(TContext context, StateMap stateMap) : base(context, stateMap)
    {
    }
}
