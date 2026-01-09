// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators;

/// <summary>
/// Operator management to handle operators
/// </summary>
public interface IOperatorManagement
{
    /// <summary>
    /// Current list of operators
    /// </summary>
    IReadOnlyList<Operator> Operators { get; }

    /// <summary>
    /// Creates an operator
    /// </summary>
    /// <param name="operator">Operator object</param>
    void AddOperator(Operator @operator);

    /// <summary>
    /// Updates an operator
    /// </summary>
    /// <param name="operator">Operator object</param>
    void UpdateOperator(Operator @operator);

    /// <summary>
    /// Delete the operator with the given identifier
    /// </summary>
    /// <param name="identifier">Identifier of the operator</param>
    void DeleteOperator(string identifier);

    /// <summary>
    /// Event to inform that an operator has changed
    /// </summary>
    event EventHandler<OperatorChangedEventArgs> OperatorChanged;
}