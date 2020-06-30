// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Model.Tests
{
    public interface ICreateUnknownPropertyRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(string unknown);
    }
}
