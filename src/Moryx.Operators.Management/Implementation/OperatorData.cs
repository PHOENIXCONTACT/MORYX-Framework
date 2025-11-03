// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Management;

internal class OperatorData : IPersistentObject
{
    public InternalOperator Operator { get; private set; }

    public OperatingUser User { get; private set; }

    public long Id { get; set; }

    public virtual string Identifier
    {
        get => Operator.Identifier; set
        {
            Operator.Identifier = value;
            User.Identifier = value;
        }
    }

    public virtual string? FirstName
    {
        get => Operator.FirstName; set
        {
            Operator.FirstName = value;
            User.FirstName = value;
        }
    }

    public virtual string? LastName
    {
        get => Operator.LastName; set
        {
            Operator.LastName = value;
            User.LastName = value;
        }
    }

    public virtual string? Pseudonym
    {
        get => Operator.Pseudonym;
        set => Operator.Pseudonym = value;
    }

    public virtual List<IOperatorAssignable> AssignedResources
        => Operator.AssignedResources;

    public OperatorData(string identifier)
    {
        Operator = new InternalOperator(identifier);
        User = new OperatingUser(Operator.AssignedResources) { Identifier = identifier };
    }
}
