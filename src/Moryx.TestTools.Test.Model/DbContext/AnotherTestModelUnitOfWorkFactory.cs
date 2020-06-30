// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.PostgreSQL;

namespace Moryx.TestTools.Test.Model
{
    /// <summary>
    /// Factory to get a unit of work for the TestModel model
    /// </summary>
    [ModelFactory(AnotherTestModelConstants.Namespace, TestModelConstants.Namespace)]
    public sealed class AnotherTestModelUnitOfWorkFactory : UnitOfWorkFactoryBase<AnotherTestModelContext, NpgsqlModelConfigurator>
    {
        protected override void Configure()
        {
            RegisterRepository<IAnotherEntityRepository>();
        }
    }
}
