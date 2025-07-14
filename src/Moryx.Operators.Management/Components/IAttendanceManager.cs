// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Operators.Management;

/// <summary>
/// Operator manager to handle involved operators during the production
/// </summary>
internal interface IAttendanceManager : IPlugin
{
    /// <summary>
    /// Default operator if no other operators available
    /// </summary>
    OperatorData? DefaultOperator { get; }

    /// <summary>
    /// Sign in an operator
    /// </summary>
    void SignIn(OperatorData @operator, IOperatorAssignable resource);

    /// <summary>
    /// Sign out an operator
    /// </summary>
    void SignOut(OperatorData @operator, IOperatorAssignable resource);

    /// <summary>
    /// Event to inform that an operator was signed in
    /// </summary>
    event EventHandler<OperatorData> OperatorSignedIn;

    /// <summary>
    /// Event to inform that an operator was signed out
    /// </summary>
    event EventHandler<OperatorData> OperatorSignedOut;
}

