// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Model.Repositories;

namespace Moryx.Model.Tests
{
    public interface ICreateWrongReturnTypeRepository : IRepository<SomeEntity>
    {
        Type Create(string wrong);
    }
}
