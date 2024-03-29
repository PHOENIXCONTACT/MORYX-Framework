// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model.Tests
{
    public interface ICreateAllParamsRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(int value, string name, int value2, int value3, int value4);
    }
}
