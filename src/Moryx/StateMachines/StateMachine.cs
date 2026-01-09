// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines;

/// <summary>
/// Class for creating and handling state machines
/// </summary>
public static class StateMachine
{
    /// <summary>
    /// Prepare fluent API to create a state machine. Call <see cref="TypedContextWrapper{TContext}.With{TState}()"/>
    /// to finalize the operation. It will start in the state marked as <see cref="StateDefinitionAttribute.IsInitial"/>.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <typeparam name="TContext">Type of the context class</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
    /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
    /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
    /// <example>
    /// This sample shows how to define states in the state base
    /// <code>
    /// internal abstract class MyStateBase : StateBase{MyContext}
    /// {
    ///     protected MyStateBase(MyContext context, StateMap stateMap) : base(context, stateMap)
    ///     {
    ///     }
    ///
    ///     [StateDefinition(typeof(AState), IsInitial = true)]
    ///     protected const int StateA = 10;
    ///
    ///     [StateDefinition(typeof(BState))]
    ///     protected const int StateB = 20;
    ///
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// This sample shows how to create the state machine instance
    /// <code>
    /// public class SomeComponent : IStateContext
    /// {
    ///     public void Sample()
    ///     {
    ///         // Initialize a state machine
    ///         StateMachine.ForContext(this).With&lt;MyStateBase&gt;();
    ///     }
    /// }
    /// </code>
    /// </example>
    public static TypedContextWrapper<TContext> ForContext<TContext>(TContext context)
        where TContext : IStateContext

    {
        return new TypedContextWrapper<TContext>(context);
    }

    /// <summary>
    /// Prepare fluent API to create a state machine. Call <see cref="TypedContextWrapper{TContext}.With{TState}()"/>
    /// to finalize the operation. It will start in the state marked as <see cref="StateDefinitionAttribute.IsInitial"/>.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <typeparam name="TContext">Type of the context class</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
    /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
    /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
    /// <example>
    /// This sample shows how to define states in the state base
    /// <code>
    /// internal abstract class MyAsyncStateBase : AsyncStateBase{MyContext}
    /// {
    ///     protected MyAsyncStateBase(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
    ///     {
    ///     }
    ///
    ///     [StateDefinition(typeof(AState), IsInitial = true)]
    ///     protected const int StateA = 10;
    ///
    ///     [StateDefinition(typeof(BState))]
    ///     protected const int StateB = 20;
    ///
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// This sample shows how to create the state machine instance
    /// <code>
    /// public class SomeComponent : IAsyncStateContext
    /// {
    ///     public async Task SampleAsync()
    ///     {
    ///         // Initialize a state machine
    ///         await StateMachine.ForAsyncContext(this).WithAsync&lt;MyStateBase&gt;();
    ///     }
    /// }
    /// </code>
    /// </example>
    public static AsyncTypedContextWrapper<TContext> ForAsyncContext<TContext>(TContext context)
        where TContext : IAsyncStateContext
    {
        return new AsyncTypedContextWrapper<TContext>(context);
    }

    /// <summary>
    /// Will return the complete state machine as a string
    /// </summary>
    /// <example>
    /// Current: [AState (10)] - All: [AState (10)], [BState (20)], [CState (30)]
    /// </example>
    public static string Dump(StateBase currentState)
    {
        var stateMap = currentState.GetMap();
        var states = stateMap.Select(dict => $"[{dict.Value.ToString()}]");

        var allStates = string.Join(", ", states);
        return $"Current: [{currentState}] - All: {allStates}";
    }

    /// <summary>
    /// Forces a given state on the given StateMachine.
    /// </summary>
    /// <param name="current">The current StateMachine</param>
    /// <param name="forceState">The target state to be forced</param>
    /// <example>
    /// This sample shows how to force a state
    /// <code>
    /// public class SomeComponent : IStateContext
    /// {
    ///     public void Sample()
    ///     {
    ///         StateMachine.Force(context.State, MyStateBase.StateA);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void Force(SyncStateBase current, int forceState)
    {
        Force(current, forceState, true, true);
    }

    /// <summary>
    /// Forces a given state on the given StateMachine.
    /// </summary>
    /// <param name="current">The current StateMachine</param>
    /// <param name="forceState">The target state to be forced</param>
    /// <param name="exitCurrent">If <c>true</c> <see cref="SyncStateBase.OnExit"/> will be called before force.</param>
    /// <param name="enterForced">If <c>true</c> <see cref="SyncStateBase.OnEnter"/> will be called on forced state.</param>
    /// <example>
    /// This sample shows how to force a state
    /// <code>
    /// public class SomeComponent : IStateContext
    /// {
    ///     public void Sample()
    ///     {
    ///         StateMachine.Force(context.State, MyStateBase.StateA, exitCurrent: true, enterForced: false);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void Force(SyncStateBase current, int forceState, bool exitCurrent, bool enterForced)
    {
        current.Force(forceState, exitCurrent, enterForced);
    }

    /// <summary>
    /// Forces a given state on the given StateMachine.
    /// </summary>
    /// <param name="current">The current StateMachine</param>
    /// <param name="forceState">The target state to be forced</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <example>
    /// This sample shows how to force a state
    /// <code>
    /// public class SomeComponent : IStateContext
    /// {
    ///     public async Task SampleAsync()
    ///     {
    ///         await StateMachine.ForceAsync(context.State, MyStateBase.StateA);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static Task ForceAsync(AsyncStateBase current, int forceState, CancellationToken cancellationToken = default)
    {
        return ForceAsync(current, forceState, true, true, cancellationToken);
    }

    /// <summary>
    /// Forces a given state on the given StateMachine.
    /// </summary>
    /// <param name="current">The current StateMachine</param>
    /// <param name="forceState">The target state to be forced</param>
    /// <param name="exitCurrent">If <c>true</c> <see cref="AsyncStateBase.OnExitAsync"/> will be called before force.</param>
    /// <param name="enterForced">If <c>true</c> <see cref="AsyncStateBase.OnEnterAsync"/> will be called on forced state.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <example>
    /// This sample shows how to force a state
    /// <code>
    /// public class SomeComponent : IStateContext
    /// {
    ///     public async Task SampleAsync()
    ///     {
    ///         await StateMachine.ForceAsync(context.State, MyStateBase.StateA, exitCurrent: true, enterForced: false);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static Task ForceAsync(AsyncStateBase current, int forceState, bool exitCurrent, bool enterForced, CancellationToken cancellationToken = default)
    {
        return current.ForceAsync(forceState, exitCurrent, enterForced, cancellationToken);
    }

    /// <summary>
    /// Wrapper for the context class to use generic language features
    /// and provide fluent API
    /// </summary>
    public readonly struct AsyncTypedContextWrapper<TContext>
    {
        private readonly TContext _context;

        /// <summary>
        /// Create context wrapper
        /// </summary>
        internal AsyncTypedContextWrapper(TContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <typeparam name="TStateBase">Type of state machine base class</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public Task WithAsync<TStateBase>(CancellationToken cancellationToken = default)
            where TStateBase : AsyncStateBase
        {
            return AsyncStateBase.CreateAsync(typeof(TStateBase), (IAsyncStateContext)_context, null, cancellationToken);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <typeparam name="TStateBase">Type of state machine base class</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public Task WithAsync<TStateBase>(int initialState, CancellationToken cancellationToken = default)
            where TStateBase : AsyncStateBase
        {
            return AsyncStateBase.CreateAsync(typeof(TStateBase), (IAsyncStateContext)_context, initialState, cancellationToken);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <param name="stateType">Type of state machine base class</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public Task WithAsync(Type stateType, CancellationToken cancellationToken = default)
        {
            return AsyncStateBase.CreateAsync(stateType, (IAsyncStateContext)_context, null, cancellationToken);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <param name="initialState">Initial state of the state machine. Used for reloads.</param>
        /// <param name="stateType">Type of state machine base class</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public Task WithAsync(Type stateType, int initialState, CancellationToken cancellationToken = default)
        {
            return AsyncStateBase.CreateAsync(stateType, (IAsyncStateContext)_context, initialState, cancellationToken);
        }
    }

    /// <summary>
    /// Wrapper for the context class to use generic language features
    /// and provide fluent API
    /// </summary>
    public readonly struct TypedContextWrapper<TContext>
    {
        private readonly TContext _context;

        /// <summary>
        /// Create context wrapper
        /// </summary>
        internal TypedContextWrapper(TContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <typeparam name="TStateBase">Type of state machine base class</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public void With<TStateBase>()
            where TStateBase : SyncStateBase
        {
            SyncStateBase.Create(typeof(TStateBase), (IStateContext)_context, null);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <param name="initialState">Initial state of the state machine. Used for reloads.</param>
        /// <typeparam name="TStateBase">Type of state machine base class</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public void With<TStateBase>(int initialState)
            where TStateBase : SyncStateBase
        {
            SyncStateBase.Create(typeof(TStateBase), (IStateContext)_context, initialState);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <param name="stateType">Type of state machine base class</param>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public void With(Type stateType)
        {
            SyncStateBase.Create(stateType, (IStateContext)_context, null);
        }

        /// <summary>
        /// Finalize the state machine creation by defining the state machine type.
        /// </summary>
        /// <param name="stateType">Type of state machine base class</param>
        /// <param name="initialState">Initial state of the state machine. Used for reloads.</param>
        /// <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        /// <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        /// <exception cref="ArgumentException">Given base class is not abstract.</exception>
        public void With(Type stateType, int initialState)
        {
            SyncStateBase.Create(stateType, (IStateContext)_context, initialState);
        }
    }
}
