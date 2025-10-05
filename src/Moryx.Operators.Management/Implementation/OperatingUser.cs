// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Users;
namespace Moryx.Operators.Management;

internal class OperatingUser(IReadOnlyList<IOperatorAssignable> assignedResources) : User
{
    public new string Identifier
    {
        get => base.Identifier;
        set => base.Identifier = value;
    } 

    public new string? FirstName
    {
        get => base.FirstName;
        set => base.FirstName = value;
    }

    public new string? LastName
    {
        get => base.LastName;
        set => base.LastName = value;
    }

    public override bool SignedIn { 
        get => AssignedResources.Any(); 
        protected set => throw new InvalidOperationException();
    }

    public IReadOnlyList<IOperatorAssignable> AssignedResources { get; private set; } = assignedResources;
}

