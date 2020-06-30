// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.InMemory;
using Moryx.TestTools.Test.Model;

namespace Moryx.Model.Tests
{
    internal class UnitOfWorkFactoryMock : InMemoryUnitOfWorkFactoryBase<TestModelContext>
    {
        protected override void Configure()
        {
            RegisterRepository<ICarEntityRepository>(); // Proxy
            RegisterRepository<ISportCarRepository, SportCarRepository>(); // Proxy
            RegisterRepository<IHouseEntityRepository, HouseEntityRepository>(true); // No Proxy
        }

        public UnitOfWorkFactoryMock(string instanceId) : base(instanceId)
        {
        }
    }
}
