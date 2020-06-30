// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.PostgreSQL;

namespace Moryx.TestTools.Test.Model
{
    /// <summary>
    /// Used for inherited models
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class TestModelUnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext, NpgsqlModelConfigurator>
        where TContext : TestModelContext
    {

    }

    /// <summary>
    /// Factory to get a unit of work for the TestModel model
    /// </summary>
    [ModelFactory(TestModelConstants.Namespace)]
    public sealed class TestModelUnitOfWorkFactory : TestModelUnitOfWorkFactory<TestModelContext>
    {
        protected override void Configure()
        {
            RegisterRepository<ICarEntityRepository>();
            RegisterRepository<ISportCarRepository, SportCarRepository>();
            RegisterRepository<IWheelEntityRepository>();
            RegisterRepository<IJsonEntityRepository>();
            RegisterRepository<IHouseEntityRepository, HouseEntityRepository>(true);
        }
    }
}
