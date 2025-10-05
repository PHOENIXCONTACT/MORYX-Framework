// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators;

/// <summary>
/// Operator Data class with the operator information
/// </summary>
/// <remarks>
/// Creates a new AssignableOperator
/// </remarks>
/// <param name="identifier">The identifier of the operator</param>
public class AssignableOperator(string identifier) : Operator(identifier)
{
    /// <summary>
    /// Flag if the operator is currently signed in
    /// </summary>
    public virtual bool SignedIn => AssignedResources.Any();

    /// <summary>
    /// Id of the resource, which the operator is assigned to
    /// </summary>
    public virtual IReadOnlyList<IOperatorAssignable> AssignedResources { get; protected set; } = [];
}