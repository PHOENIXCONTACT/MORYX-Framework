// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Model.Tests
{
    public interface ICreateWrongReturnTypeRepository : IRepository<SomeEntity>
    {
        Type Create(string wrong);
    }
}
