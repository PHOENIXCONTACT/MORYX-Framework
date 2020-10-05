// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Moryx.Logging;

namespace Moryx.StateMachines
{
    /// <summary>
    /// Base class for state machine states
    /// </summary>
    public abstract class StateBase : IState
    {
        /// <summary>
        /// Map of state names and their object reference
        /// </summary>
        protected StateMap Map { get; }

        /// <summary>
        /// Instance representation of the state key
        /// </summary>
        public int Key { get; private set; }

        /// <summary>
        /// Context of the state machine
        /// </summary>
        protected IStateContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateBase"/> class.
        /// </summary>
        /// <param name="context">Context of the state machine</param>
        /// <param name="stateMap">Map of states to objects</param>
        protected StateBase(IStateContext context, StateMap stateMap)
        {
            Map = stateMap;
            Context = context;
        }

        /// <see cref="IState"/>
        public virtual void OnEnter()
        {

        }

        /// <see cref="IState"/>
        public virtual void OnExit()
        {

        }

        /// <summary>
        /// Throws an exception that the current state is invalid.
        /// </summary>
        protected void InvalidState([CallerMemberName]string methodName = "")
        {
            throw CreateAndLogInvalidStateException(methodName);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that has completed with a InvalidOperationException exception.
        /// </summary>
        protected Task InvalidStateAsync([CallerMemberName] string methodName = "")
        {
#if HAVE_TASK_FROMEXCEPTION
            return Task.FromException(CreateAndLogInvalidStateException(methodName));
#else
            var tcs = new TaskCompletionSource<int>();
            tcs.SetException(CreateAndLogInvalidStateException(methodName));
            return tcs.Task;
#endif
        }

        /// <summary>
        /// Creates a typed <see cref="Task{TResult}" /> that has completed with a InvalidOperationException exception.
        /// </summary>
        protected Task<T> InvalidStateAsync<T>([CallerMemberName] string methodName = "")
        {
#if HAVE_TASK_FROMEXCEPTION
            return Task.FromException<T>(CreateAndLogInvalidStateException(methodName));
#else
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(CreateAndLogInvalidStateException(methodName));
            return tcs.Task;
#endif
        }

        /// <summary>
        /// Creates a new InvalidOperationException and logs it to the context
        /// </summary>
        private Exception CreateAndLogInvalidStateException(string methodName)
        {
            var error = $"The state with the name '{GetType().Name}' cannot handle the method '{methodName}'.";

            // ReSharper disable once SuspiciousTypeConversion.Global
            (Context as ILoggingComponent)?.Logger.Log(LogLevel.Error, error);
            return new InvalidOperationException(error);
        }

        /// <summary>
        /// Create a state machine of the given base type and will set it on the given context
        /// Will internally called by the <see cref="StateMachine"/> wrapper class
        /// </summary>
        internal static void Create<TStateBase>(IStateContext context, int? initialKey)
            where TStateBase : StateBase
        {
            var stateBaseType = typeof(TStateBase);

            // Check the base type
            if (!stateBaseType.IsAbstract)
                throw new ArgumentException("The state base class must be abstract!");

            // Load all fields
            // 1. Get all fields which are static constant with the attribute
            // 2. let attribute and create an anonymous array
            var definedStates = (from stateField in GetStateFields(stateBaseType)
                                 let att = stateField.GetCustomAttribute<StateDefinitionAttribute>()
                                 select new { Key = (int)stateField.GetValue(null), att.IsInitial, att.Type }).ToArray();

            if (definedStates.Length == 0)
                throw new InvalidOperationException("There was no state constant defined in the given base type." +
                                                    $"There musst be at least one constant integer attributed with the {nameof(StateDefinitionAttribute)}.");

            // If a initial key is set, we check if it exists
            if (initialKey.HasValue && definedStates.All(s => s.Key != initialKey.Value))
                throw new InvalidOperationException($"There was no state defined with key: {initialKey}");

            // Group by type to find multiple defined state types
            var duplicates = definedStates.GroupBy(state => state.Type).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
            if (duplicates.Any())
            {
                var typeNames = string.Join(", ", duplicates.Select(type => type.Name));
                throw new InvalidOperationException($"State types are only allowed once: {typeNames}");
            }
            
            var stateMap = new StateMap();
            StateBase initialState = null;
            foreach (var definedState in definedStates)
            {
                var instance =  (StateBase) Activator.CreateInstance(definedState.Type, context, stateMap);
                instance.Key = definedState.Key;
                
                if (initialKey.HasValue && initialKey.Value == definedState.Key)
                    initialState = instance;
                else if (definedState.IsInitial && initialState == null)
                    initialState = instance;
                else if (definedState.IsInitial && initialState != null)
                    throw new InvalidOperationException("At least one state must be flagged as '" +
                                                        $"{nameof(StateDefinitionAttribute.IsInitial)} = true'.");

                stateMap.Add(definedState.Key, instance);
            }

            if (initialState == null)
                throw new InvalidOperationException("There is no state flagged with " +
                                                    $"'{nameof(StateDefinitionAttribute.IsInitial)} = true'.");

            context.SetState(initialState);
            initialState.OnEnter();
        }

        /// <summary>
        /// Returns all fields of the given Type of <see cref="StateBase"/> which are attributed with the <see cref="StateDefinitionAttribute"/>
        /// </summary>
        internal static IEnumerable<FieldInfo> GetStateFields(Type stateBaseType)
        {
            var stateFields = from field in stateBaseType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                where field.IsLiteral && !field.IsInitOnly &&
                      field.FieldType.IsAssignableFrom(typeof(int)) &&
                      field.GetCustomAttribute<StateDefinitionAttribute>() != null
                select field;
            return stateFields;
        }

        /// <summary>
        /// Forces a specific state with option to exit the current and enter the forced state
        /// </summary>
        internal void Force(int state, bool exitCurrent, bool enterForced)
        {
            if (!Map.ContainsKey(state))
            {
                throw new InvalidOperationException("State cannot be forced. " +
                                                    $"The state {state} does not exist in StateMachine.");
            }

            // If requested, exit current state
            if (exitCurrent)
                OnExit();
            
            var next = Map[state];
            Context.SetState(next);

            // If requested, enter forced state
            if (enterForced)
                next.OnEnter();
        }

        /// <summary>
        /// Will return the protected map.
        /// Will internally called by the <see cref="StateMachine"/> wrapper class
        /// </summary>
        internal StateMap GetMap()
        {
            return Map;
        }

        /// <summary>
        /// Jump to next state
        /// </summary>
        /// <param name="state">Number of the next state</param>
        protected virtual void NextState(int state)
        {
            if (!Map.ContainsKey(state))
            {
                throw new InvalidOperationException($"The state {state} does not exist in StateMachine.");
            }

            // Exit the old state
            OnExit();

            // Set the next state
            var next = Map[state];
            Context.SetState(next);

            // Enter the next state
            next.OnEnter();
        }

        /// <summary>
        /// String representation of this state. Will print the key and name of the state
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name} ({Key})";
        }

        /// <summary>
        /// Shortcut class for the statemap dictionary
        /// </summary>
        public sealed class StateMap : Dictionary<int, IState>
        {
        }
    }

    /// <summary>
    /// Base class for state machine states 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class StateBase<TContext> : StateBase
        where TContext : IStateContext
    {
        /// <summary>
        /// Context of the state machine
        /// </summary>
        protected new TContext Context => (TContext) base.Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateBase"/> class.
        /// </summary>
        /// <param name="stateMap">Map of states to objects</param>
        /// <param name="context">Context of the state machine</param>
        protected StateBase(TContext context, StateMap stateMap) : base(context, stateMap)
        {
        }
    }
}
