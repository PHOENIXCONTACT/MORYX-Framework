// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Operators.Management;

/// <summary>
/// Operator manager to handle involved operators during the production
/// </summary>
internal interface IOperatorManager : IPlugin
{
    /// <summary>
    /// Current operator information
    /// </summary>
    IReadOnlyList<OperatorData> Operators { get; }

    /// <summary>
    /// Add an operator
    /// </summary>
    OperatorData Add(Operator @operator);

    /// <summary>
    /// Update an operator
    /// </summary>
    void Update(Operator @operator);

    /// <summary>
    /// Delete an operator
    /// </summary>
    void Delete(string identifier);

    /// <summary>
    /// Event to inform that an operator changed
    /// </summary>
    event EventHandler<OperatorChangedEventArgs> OperatorChanged;
}

