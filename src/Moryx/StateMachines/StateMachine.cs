// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;

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
        /// Forces a given state state on the given StateMachine.
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
        public static void Force(StateBase current, int forceState)
        {
            Force(current, forceState, true, true);
        }

        /// <summary>
        /// Forces a given state state on the given StateMachine.
        /// </summary>
        /// <param name="current">The current StateMachine</param>
        /// <param name="forceState">The target state to be forced</param>
        /// <param name="exitCurrent">If <c>true</c> <see cref="IState.OnExit"/> will be called before force.</param>
        /// <param name="enterForced">If <c>true</c> <see cref="IState.OnEnter"/> will be called on forced state.</param>
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
        public static void Force(StateBase current, int forceState, bool exitCurrent, bool enterForced)
        {
            current.Force(forceState, exitCurrent, enterForced);
        }

        /// <summary>
        /// Wrapper for the context class to use generic language features
        /// and provide fluent API
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        public struct TypedContextWrapper<TContext>
            where TContext : IStateContext
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
                where TStateBase : StateBase
            {
                StateBase.Create<TStateBase>(_context, _key);
            }
        }
    }
}
