// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines;

/// <summary>
/// Base class for asynchronous state machine states
/// </summary>
public abstract class AsyncStateBase : StateBase
{
    /// <summary>
    /// Context of the state machine
    /// </summary>
    protected new IAsyncStateContext Context => (IAsyncStateContext)base.Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateBase"/> class.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <param name="stateMap">Map of states to objects</param>
    protected AsyncStateBase(IAsyncStateContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    /// <summary>
    /// Will be called while entering the next state
    /// </summary>
    public virtual Task OnEnterAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Will be called while exiting the current state
    /// </summary>
    public virtual Task OnExitAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Jump to next state async
    /// </summary>
    /// <param name="state">Number of the next state</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    protected virtual async Task NextStateAsync(int state, CancellationToken cancellationToken = default)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException($"The state {state} does not exist in StateMachine.");
        }

        // Exit the old state
        await OnExitAsync(cancellationToken);

        // Set next state
        await Context.SetStateAsync(next, cancellationToken);

        // Enter the next state
        await ((AsyncStateBase)next).OnEnterAsync(cancellationToken);
    }

    /// <summary>
    /// Forces a specific state with option to exit the current and enter the forced state
    /// </summary>
    internal virtual async Task ForceAsync(int state, bool exitCurrent, bool enterForced, CancellationToken cancellationToken)
    {
        if (!Map.TryGetValue(state, out var next))
        {
            throw new InvalidOperationException("State cannot be forced. " +
                                                $"The state {state} does not exist in StateMachine.");
        }

        var nextState = (AsyncStateBase)next;

        // If requested, exit current state
        if (exitCurrent)
            await OnExitAsync(cancellationToken);

        // Set next state
        await Context.SetStateAsync(nextState, cancellationToken);

        // If requested, enter forced state
        if (enterForced)
            await nextState.OnEnterAsync(cancellationToken);
    }

    /// <summary>
    /// Create a state machine of the given base type and will set it on the given context
    /// Will internally called by the <see cref="StateMachine"/> wrapper class
    /// </summary>
    internal static async Task CreateAsync(Type stateBaseType, IAsyncStateContext context, int? initialKey, CancellationToken cancellationToken)
    {
        if (!typeof(AsyncStateBase).IsAssignableFrom(stateBaseType))
            throw new InvalidOperationException($"Only states inherited from {nameof(AsyncStateBase)} are supported!");

        var initialState = CreateMapAndGetInitial(stateBaseType, context, initialKey) as AsyncStateBase;
        if (initialState == null)
            throw new ArgumentException($"Initial state does not inherit from {nameof(AsyncStateBase)}");

        await context.SetStateAsync(initialState, cancellationToken);
        await initialState.OnEnterAsync(cancellationToken);
    }
}

/// <summary>
/// Base class for asynchronous state machine states
/// </summary>
/// <typeparam name="TContext">Typed context</typeparam>
public abstract class AsyncStateBase<TContext> : AsyncStateBase
    where TContext : IAsyncStateContext
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
