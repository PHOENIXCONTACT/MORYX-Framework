// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Model;
using Marvin.Model.InMemory;

namespace Marvin.TestTools.Test.Model
{
    [ModelFactory(AnotherTestModelConstants.Namespace, TestModelConstants.Namespace)]
    public class AnotherInMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<AnotherTestModelContext>
    {
        public AnotherInMemoryUnitOfWorkFactory() : base(Guid.NewGuid().ToString())
        {
        }

        public AnotherInMemoryUnitOfWorkFactory(string instanceId) : base(instanceId)
        {
        }
        
        protected override void Configure()
        {
            RegisterRepository<IAnotherEntityRepository>();
        }
    }
}
