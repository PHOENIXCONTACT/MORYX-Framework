// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base class for states with typed context object
    /// </summary>
    /// <typeparam name="TContext">Type of the driver context</typeparam>
    public abstract class DriverState<TContext> : SyncStateBase<TContext>, IDriverState
        where TContext : Driver
    {
        /// <inheritdoc />
        public StateClassification Classification { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverState{TContext}"/> class.
        /// </summary>
        /// <param name="classification">The classification of the state.</param>
        /// <param name="stateMap">Map of states to objects</param>
        /// <param name="context">Context of the state machine</param>
        protected DriverState(TContext context, StateMap stateMap, StateClassification classification) : base(context, stateMap)
        {
            Classification = classification;
        }

        /// <summary>
        /// State transition to connect
        /// </summary>
        public virtual void Connect()
        {
            InvalidState();
        }

        /// <summary>
        /// State transition to disconnect
        /// </summary>
        public virtual void Disconnect()
        {
            InvalidState();
        }
    }
}
