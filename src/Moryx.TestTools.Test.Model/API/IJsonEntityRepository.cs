// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    public interface IJsonEntityRepository : IRepository<JsonEntity>
    {
        JsonEntity Create(string jsonData);
    }
}
