﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Management.Implementation;

internal class InternalOperator : AssignableOperator
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

    public new string? Pseudonym
    {
        get => base.Pseudonym;
        set => base.Pseudonym = value;
    }

    public new List<IOperatorAssignable> AssignedResources
    {
        get => (List<IOperatorAssignable>)base.AssignedResources; 
        private set => base.AssignedResources = value;
    }

    public InternalOperator(string identifier) : base(identifier) => AssignedResources = [];
}

