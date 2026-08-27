// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Moryx.Logging;

namespace Moryx.StateMachines;

/// <summary>
/// Base class for state machine states
/// </summary>
public abstract class StateBase
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
    protected object Context { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateBase"/> class.
    /// </summary>
    /// <param name="context">Context of the state machine</param>
    /// <param name="stateMap">Map of states to objects</param>
    protected StateBase(object context, StateMap stateMap)
    {
        Map = stateMap;
        Context = context;
    }

    /// <summary>
    /// Throws an exception that the current state is invalid.
    /// </summary>
    protected void InvalidState([CallerMemberName] string methodName = "")
    {
        throw CreateAndLogInvalidStateException(methodName);
    }

    /// <summary>
    /// Creates a <see cref="Task" /> that has completed with a InvalidOperationException exception.
    /// </summary>
    protected Task InvalidStateAsync([CallerMemberName] string methodName = "")
    {
        return Task.FromException(CreateAndLogInvalidStateException(methodName));
    }

    /// <summary>
    /// Creates a typed <see cref="Task{TResult}" /> that has completed with a InvalidOperationException exception.
    /// </summary>
    protected Task<T> InvalidStateAsync<T>([CallerMemberName] string methodName = "")
    {
        return Task.FromException<T>(CreateAndLogInvalidStateException(methodName));
    }

    /// <summary>
    /// Creates a new InvalidOperationException and logs it to the context
    /// </summary>
    private InvalidOperationException CreateAndLogInvalidStateException(string methodName)
    {
        var contextInfo = Context is IPersistentObject persistentObject
            ? $"{Context.GetType().Name} (Id = {persistentObject.Id})"
            : Context.GetType().Name;

        var error = $"The state '{GetType().Name}' cannot handle the method '{methodName}'. Responsible context: '{contextInfo}'.";

        // ReSharper disable once SuspiciousTypeConversion.Global
        (Context as ILoggingComponent)?.Logger.Log(LogLevel.Error, error);
        return new InvalidOperationException(error);
    }


    private record StateDefinition(int Key, bool IsInitial, Type Type);
    /// <summary>
    /// Create a state machine of the given base type. Returns the initial state after initialization.
    /// </summary>
    protected static StateBase CreateMapAndGetInitial(Type stateBaseType, object context, int? initialKey)
    {
        // Check the base type
        if (!stateBaseType.IsAbstract)
        {
            throw new ArgumentException("The state base class must be abstract!");
        }

        if (!typeof(StateBase).IsAssignableFrom(stateBaseType))
        {
            throw new ArgumentException($"'{stateBaseType.Name}' class is not a valid 'StateBase'!");
        }

        // Load all fields
        // 1. Get all fields which are static constant with the attribute
        // 2. let attribute and create an anonymous array
        StateDefinition[] definedStates =
            (from stateField in GetStateFields(stateBaseType)
             let att = stateField.GetCustomAttribute<StateDefinitionAttribute>()
             select new StateDefinition((int)stateField.GetValue(null), att.IsInitial, att.Type)).ToArray();

        ValidateStateDefinitions(definedStates, initialKey);

        var stateMap = new StateMap();
        StateBase initialState = null;
        foreach (var definedState in definedStates)
        {
            var instance = Activator.CreateInstance(definedState.Type, context, stateMap) as StateBase
                ?? throw new InvalidOperationException($"Could not create instance of State type {definedState.Type.Name}");

            instance.Key = definedState.Key;

            if ((initialKey.HasValue && initialKey.Value == definedState.Key)
                || (!initialKey.HasValue && definedState.IsInitial))
            {
                initialState = instance;
            }

            stateMap.Add(definedState.Key, instance);
        }

        return initialState;
    }

    private static void ValidateStateDefinitions(StateDefinition[] definedStates, int? initialKey)
    {
        if (definedStates.Length == 0)
        {
            throw new InvalidOperationException("There was no state constant defined in the given base type." +
                                                $"There must be at least one constant integer attributed with the {nameof(StateDefinitionAttribute)}.");
        }

        if (initialKey.HasValue)
        {
            // If an initial key is set, we check if it exists
            if (definedStates.All(s => s.Key != initialKey.Value))
            {
                throw new InvalidOperationException($"There was no state defined with key: {initialKey}");
            }
        }
        else
        {
            // Otherwise we check that exactly one key is marked as initial
            var initialStates = definedStates.Where(s => s.IsInitial).ToArray();
            if (initialStates.Length == 0)
            {
                throw new InvalidOperationException("No state is marked as initial. Set one explicitly using the StateDefinitionAttribute or the ");
            }
            else if (initialStates.Length > 1)
            {
                var initialStateString = string.Join(", ", initialStates.Select(i => i.Type.Name));
                throw new InvalidOperationException($"Multipe states are marked as initial: '{initialStateString}'. Define exactly one or set an initial state explicitly");
            }
        }

        // Group by type to find states types that are used multiple times
        var duplicateStates = definedStates
            .GroupBy(state => state.Type)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();
        if (duplicateStates.Any())
        {
            var typeNames = string.Join(", ", duplicateStates.Select(type => type.Name));
            throw new InvalidOperationException($"State types are only allowed once: {typeNames}");
        }
        // Group by key to find statemachine keys that are used multiple times
        var duplicateKeys = definedStates
            .GroupBy(state => state.Key)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();
        if (duplicateKeys.Any())
        {
            var stateKeys = string.Join(", ", duplicateKeys);
            throw new InvalidOperationException($"State keys are only allowed once: {stateKeys}");
        }
    }

    /// <summary>
    /// Returns all fields of the given Type of <see cref="StateBase"/> which are attributed with the <see cref="StateDefinitionAttribute"/>
    /// </summary>
    internal static IEnumerable<FieldInfo> GetStateFields(Type stateBaseType)
    {
        var stateFields =
            from field in stateBaseType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            where field.IsLiteral && !field.IsInitOnly &&
                  field.FieldType.IsAssignableFrom(typeof(int)) &&
                  field.GetCustomAttribute<StateDefinitionAttribute>() != null
            select field;
        return stateFields;
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
    /// String representation of this state. Will print the key and name of the state
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} ({Key})";
    }

    /// <summary>
    /// Shortcut class for the stateMap dictionary
    /// </summary>
    public sealed class StateMap : Dictionary<int, StateBase>;
}
