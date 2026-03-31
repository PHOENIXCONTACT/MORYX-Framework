// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Attendances;

public enum SignInStatus
{
    SignedOut, SignedIn
}

/// <summary>
/// Event when an operator is signed in or signed out of a resource
/// </summary>
public class SignInStatusChangedArgs : EventArgs
{
    /// <summary>
    /// New signin status
    /// </summary>
    public SignInStatus Status { get; set; }

    /// <summary>
    /// The operator that changed state
    /// </summary>
    public required AssignableOperator Operator { get; set; }

    /// <summary>
    /// The resource in which the user signed in
    /// </summary>
    public required IOperatorAssignable Assignable { get; set; }
}
