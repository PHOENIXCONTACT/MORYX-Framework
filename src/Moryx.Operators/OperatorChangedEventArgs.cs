// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators;

/// <summary>
/// EventArgs for operations that changed an operator
/// </summary>
/// <remarks>
/// Creates a new instance of the event args
/// </remarks>
/// <param name="operator">The operator that changed</param>
/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="operator"/> is null.</exception>
public class OperatorChangedEventArgs(Operator @operator) : EventArgs
{
    /// <summary>
    /// The kind of change that happened
    /// </summary>
    public OperatorChange Change { get; set; }

    /// <summary>
    /// The operator who has changed
    /// </summary>
    public Operator Operator { get; set; } = @operator ?? throw new ArgumentNullException(nameof(@operator));
}

/// <summary>
/// The kind of change an operator had
/// </summary>
public enum OperatorChange
{
    Creation,
    Update,
    Deletion
}