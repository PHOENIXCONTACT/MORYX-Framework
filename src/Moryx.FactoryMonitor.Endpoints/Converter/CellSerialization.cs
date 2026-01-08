// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Factory;
using Moryx.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Converter;

/// <summary>
/// Cell serialization class
/// </summary>
internal class CellSerialization : PossibleValuesSerialization
{
    public CellSerialization() : base(new ContainerMock(), new ServiceProviderMock(), new ValueProviderExecutor(new ValueProviderExecutorSettings()))
    {
    }

    /// <summary>
    /// Only export properties flagged with <see cref="EntryVisualizationAttribute"/>
    /// </summary>
    public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
    {
        return typeof(Resource).IsAssignableFrom(sourceType)
            ? base.GetProperties(sourceType).Where(p => p.GetCustomAttribute<EntryVisualizationAttribute>() != null)
            : new EntrySerializeSerialization().GetProperties(sourceType);
    }

    public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
    {
        return new EntrySerializeSerialization().GetMethods(sourceType);
    }

    private class ContainerMock : IContainer
    {
        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetRegisteredImplementations(Type service)
        {
            throw new NotImplementedException();
        }

        public void Register(Type type, Type[] services, string name, LifeCycle lifeCycle)
        {
            throw new NotImplementedException();
        }

        public void RegisterFactory(Type factoryInterface, string name, Type selector)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type[] services, object instance, string name)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type service, string name)
        {
            throw new NotImplementedException();
        }

        public Array ResolveAll(Type service)
        {
            throw new NotImplementedException();
        }
    }

    private class ServiceProviderMock : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}