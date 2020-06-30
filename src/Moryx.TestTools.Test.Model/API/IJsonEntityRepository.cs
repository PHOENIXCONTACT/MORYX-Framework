// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public interface IJsonEntityRepository : IRepository<JsonEntity>
    {
        JsonEntity Create(string jsonData);
    }
}
