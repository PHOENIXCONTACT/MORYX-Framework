// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.AbstractionLayer.Drivers;

/// <summary>
/// Base class for asynchronous driver states with typed context object
/// </summary>
/// <typeparam name="TContext">Type of the driver context</typeparam>
public abstract class AsyncDriverState<TContext> : AsyncStateBase<TContext>, IDriverState
    where TContext : Driver

{
    /// <inheritdoc />
    public StateClassification Classification { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDriverState{TContext}"/> class.
    /// </summary>
    /// <param name="classification">The classification of the state.</param>
    /// <param name="stateMap">Map of states to objects</param>
    /// <param name="context">Context of the state machine</param>
    protected AsyncDriverState(TContext context, StateMap stateMap, StateClassification classification) : base(context, stateMap)
    {
        Classification = classification;
    }

    /// <summary>
    /// State transition to connect
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    public virtual Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        return InvalidStateAsync();
    }

    /// <summary>
    /// State transition to disconnect
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    public virtual Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        return InvalidStateAsync();
    }
}
