// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines
{
    /// <summary>
    /// Class for creating and handling state machines
    /// </summary>
    public sealed class StateMachine
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
        ///         StateMachine.Initialize(this).With&lt;MyStateBase&gt;();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static TypedContextWrapper<TContext> Initialize<TContext>(TContext context)
            where TContext : IStateContext

        {
            return new TypedContextWrapper<TContext>(context, null);
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
        ///         await StateMachine.InitializeAsync(this).WithAsync&lt;MyStateBase&gt;();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static TypedContextWrapper<TContext> InitializeAsync<TContext>(TContext context)
            where TContext : IAsyncStateContext

        {
            return new TypedContextWrapper<TContext>(context, null);
        }

        /// <summary>
        /// Prepare fluent API to create a state machine. Call <see cref="TypedContextWrapper{TContext}.With{TState}()"/>
        /// to finalize the operation.
        /// </summary>
        /// <param name="context">Context of the state machine</param>
        /// <param name="state">Key of the state that is reloaded</param>
        /// <typeparam name="TContext">Type of the context class</typeparam>
        ///  <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        ///  <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        ///  <exception cref="ArgumentException">Given base class is not abstract.</exception>
        ///  <example>
        ///  This sample shows how to define states in the state base
        ///  <code>
        ///  internal abstract class MyStateBase : StateBase{MyContext}
        ///  {
        ///      protected MyStateBase(MyContext context, StateMap stateMap) : base(context, stateMap)
        ///      {
        ///      }
        ///
        ///      [StateDefinition(typeof(AState), IsInitial = true)]
        ///      protected const int StateA = 10;
        ///
        ///      [StateDefinition(typeof(BState))]
        ///      protected const int StateB = 20;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This sample shows how reload a state machine
        /// <code>
        /// public class SomeComponent : IStateContext
        /// {
        ///     public void Sample()
        ///     {
        ///         // Initialize a state machine
        ///         StateMachine.Initialize(this).With&lt;MyStateBase&gt;();
        ///         _state.GoToAnotherState();
        ///
        ///         // Get the key of the other state
        ///         var key = StateMachine.GetKey(_stat);
        ///
        ///         // ... save key to db or somewhere else
        ///
        ///         // Reload the state with the saved key
        ///         StateMachine.Reload(this, key).With&lt;MyStateBase&gt;();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static TypedContextWrapper<TContext> Reload<TContext>(TContext context, int state)
            where TContext : IStateContext

        {
            return new TypedContextWrapper<TContext>(context, state);
        }

        /// <summary>
        /// Prepare fluent API to create a state machine. Call <see cref="TypedContextWrapper{TContext}.WithAsync{TState}()"/>
        /// to finalize the operation.
        /// </summary>
        /// <param name="context">Context of the state machine</param>
        /// <param name="state">Key of the state that is reloaded</param>
        /// <typeparam name="TContext">Type of the context class</typeparam>
        ///  <exception cref="InvalidOperationException">Thrown if 0 or more states are flagged as initial.</exception>
        ///  <exception cref="InvalidOperationException">Thrown if types are registered more than one time.</exception>
        ///  <exception cref="ArgumentException">Given base class is not abstract.</exception>
        ///  <example>
        ///  This sample shows how to define states in the state base
        ///  <code>
        ///  internal abstract class MyAsyncStateBase : AsyncStateBase{MyContext}
        ///  {
        ///      protected MyStateBase(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
        ///      {
        ///      }
        ///
        ///      [StateDefinition(typeof(AState), IsInitial = true)]
        ///      protected const int StateA = 10;
        ///
        ///      [StateDefinition(typeof(BState))]
        ///      protected const int StateB = 20;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This sample shows how reload a state machine
        /// <code>
        /// public class SomeComponent : IStateContext
        /// {
        ///     public async Task SampleAsync()
        ///     {
        ///         // Initialize a state machine
        ///         await StateMachine.InitializeAsync(this).WithAsync&lt;MyStateBase&gt;();
        ///         await _state.GoToAnotherState();
        ///
        ///         // Get the key of the other state
        ///         var key = StateMachine.GetKey(_stat);
        ///
        ///         // ... save key to db or somewhere else
        ///
        ///         // Reload the state with the saved key
        ///         await StateMachine.ReloadAsync(this, key).WithAsync&lt;MyStateBase&gt;();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static TypedContextWrapper<TContext> ReloadAsync<TContext>(TContext context, int state)
            where TContext : IAsyncStateContext

        {
            return new TypedContextWrapper<TContext>(context, state);
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
        /// <typeparam name="TContext"></typeparam>
        public readonly struct TypedContextWrapper<TContext>
        {
            private readonly TContext _context;
            private readonly int? _key;

            /// <summary>
            /// Create context wrapper
            /// </summary>
            internal TypedContextWrapper(TContext context, int? key)
            {
                _context = context;
                _key = key;
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
                SyncStateBase.Create(typeof(TStateBase), (IStateContext)_context, _key);
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
                return AsyncStateBase.CreateAsync(typeof(TStateBase), (IAsyncStateContext)_context, _key, cancellationToken);
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
                SyncStateBase.Create(stateType, (IStateContext)_context, _key);
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
                return AsyncStateBase.CreateAsync(stateType, (IAsyncStateContext)_context, _key, cancellationToken);
            }
        }
    }
}
