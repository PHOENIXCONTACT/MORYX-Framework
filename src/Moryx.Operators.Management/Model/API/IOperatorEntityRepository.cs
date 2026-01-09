// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Operators.Management.Model;

public interface IOperatorEntityRepository : IRepository<OperatorEntity>
{
    OperatorEntity GetByIdentifier(string identifier);

    OperatorEntity Create(string firstName, string lastName, string identifier);
}

