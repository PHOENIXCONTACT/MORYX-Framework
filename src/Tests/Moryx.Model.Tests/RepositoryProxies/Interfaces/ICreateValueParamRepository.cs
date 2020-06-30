// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model.Tests
{
    public interface ICreateValueParamRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(Int32 value);
    }
}
