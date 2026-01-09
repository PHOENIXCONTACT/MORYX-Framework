// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model.Tests;

public interface ICreateUnknownPropertyRepository : IRepository<SomeEntity>
{
    SomeEntity Create(string unknown);
}