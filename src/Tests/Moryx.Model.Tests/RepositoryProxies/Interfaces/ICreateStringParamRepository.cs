// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model.Tests
{
    public interface ICreateStringParamRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(string name);
    }
}
