// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Attendances;

public enum SignInStatus
{
    SignedOut, SignedIn
}

public class SignInStatusChangedArgs : EventArgs
{
    public SignInStatus Status { get; set; }
    public required AssignableOperator Operator { get; set; }
    public required IOperatorAssignable Resource { get; set; }
}
