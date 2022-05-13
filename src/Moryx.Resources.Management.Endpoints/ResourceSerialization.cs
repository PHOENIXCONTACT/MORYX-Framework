// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moryx.Resources.Management.Endpoints
{
    internal class ResourceSerialization : PossibleValuesSerialization
    {
        public ResourceSerialization() : base(new ContainerMock(), new ValueProviderExecutor(new ValueProviderExecutorSettings()))
        {
        }

        /// <summary>
        /// Only export properties flagged with <see cref="EntrySerializeAttribute"/>
        /// </summary>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            return typeof(Resource).IsAssignableFrom(sourceType)
                ? base.GetProperties(sourceType).Where(p => p.GetCustomAttribute<EntrySerializeAttribute>()?.Mode == EntrySerializeMode.Always)
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

            public IContainer ExecuteInstaller(IContainerInstaller installer)
            {
                throw new NotImplementedException();
            }

            public void Extend<TExtension>() where TExtension : new()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Type> GetRegisteredImplementations(Type componentInterface)
            {
                throw new NotImplementedException();
            }

            public void LoadComponents<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public void LoadComponents<T>(Predicate<Type> condition) where T : class
            {
                throw new NotImplementedException();
            }

            public IContainer Register<TService, TComp>()
                where TService : class
                where TComp : TService
            {
                throw new NotImplementedException();
            }

            public IContainer Register<TService, TComp>(string name, LifeCycle lifeCycle)
                where TService : class
                where TComp : TService
            {
                throw new NotImplementedException();
            }

            public IContainer Register<TFactory>() where TFactory : class
            {
                throw new NotImplementedException();
            }

            public IContainer Register<TFactory>(string name) where TFactory : class
            {
                throw new NotImplementedException();
            }

            public T Resolve<T>()
            {
                throw new NotSupportedException();
            }

            public object Resolve(Type service)
            {
                throw new NotSupportedException();
            }

            public T Resolve<T>(string name)
            {
                throw new NotSupportedException();
            }

            public object Resolve(Type service, string name)
            {
                throw new NotSupportedException();
            }

            public T[] ResolveAll<T>()
            {
                throw new NotSupportedException();
            }

            public Array ResolveAll(Type service)
            {
                throw new NotSupportedException();
            }

            public IContainer SetInstance<T>(T instance) where T : class
            {
                throw new NotImplementedException();
            }

            public IContainer SetInstance<T>(T instance, string name) where T : class
            {
                throw new NotImplementedException();
            }
        }
    }
}
