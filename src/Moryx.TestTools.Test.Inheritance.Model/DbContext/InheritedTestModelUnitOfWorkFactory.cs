// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;
using Marvin.TestTools.Test.Model;

namespace Marvin.TestTools.Test.Inheritance.Model
{
    [ModelFactory(InheritedTestModelConstants.Name, TestModelConstants.Namespace)]
    public class InheritedTestModelUnitOfWorkFactory : TestModelUnitOfWorkFactory<InheritedTestModelContext>
    {
        protected override void Configure()
        {
            RegisterRepository<ISuperCarEntityRepository>();
        }
    }
}
