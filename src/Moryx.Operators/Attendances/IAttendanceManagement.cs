// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Attendances;

/// <summary>
/// Attandance management to handle sign in and sign out operations of involved operators during the production
/// </summary>
public interface IAttendanceManagement
{
    /// <summary>
    /// Current list of operator information
    /// </summary>
    IReadOnlyList<AssignableOperator> Operators { get; }

    /// <summary>
    /// Default operator if no other operators available
    /// </summary>
    AssignableOperator? DefaultOperator { get; }

    /// <summary>
    /// Signs in a operator
    /// </summary>
    void SignIn(AssignableOperator @operator, IOperatorAssignable resource);

    /// <summary>
    /// Signs out a operator
    /// </summary>
    void SignOut(AssignableOperator @operator, IOperatorAssignable resource);

    /// <summary>
    /// Event to inform that a operator was signed in
    /// </summary>
    event EventHandler<AssignableOperator> OperatorSignedIn;

    /// <summary>
    /// Event to inform that a operator was signed out
    /// </summary>
    event EventHandler<AssignableOperator> OperatorSignedOut;
}
