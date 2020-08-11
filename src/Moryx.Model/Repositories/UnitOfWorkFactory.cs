// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moryx.Model.Configuration;
using Moryx.Model.Repositories.Proxy;
using Moryx.Tools;

namespace Moryx.Model.Repositories
{
    /// <inheritdoc />
    public sealed class UnitOfWorkFactory<TContext> : IUnitOfWorkFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Manager that does the real work
        /// </summary>
        private readonly IDbContextManager _manager;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly RepositoryProxyBuilder ProxyBuilder = new RepositoryProxyBuilder();
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IDictionary<Type, Func<Repository>> Repositories = new Dictionary<Type, Func<Repository>>();

        /// <summary>
        /// Static constructor to register repositories once a time per typed context
        /// </summary>
        static UnitOfWorkFactory()
        {
            RegisterRepositories();
        }

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWorkFactory{TContext}"/>
        /// </summary>
        public UnitOfWorkFactory(IDbContextManager dbContextManager)
        {
            _manager = dbContextManager;
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create()
        {
            var context = _manager.Create<TContext>();
            return new UnitOfWork<TContext>(context, Repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(IDatabaseConfig config)
        {
            var context = _manager.Create<TContext>(config);
            return new UnitOfWork<TContext>(context, Repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(ContextMode contextMode)
        {
            var context = _manager.Create<TContext>(contextMode);
            return new UnitOfWork<TContext>(context, Repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(IDatabaseConfig config, ContextMode contextMode)
        {
            var context = _manager.Create<TContext>(config, contextMode);
            return new UnitOfWork<TContext>(context, Repositories);
        }

        private static void RegisterRepositories()
        {
            var types = typeof(TContext).Assembly.GetTypes();

            var repoApis = from type in types
                let genericApi = type.GetInterfaces().FirstOrDefault(i => i.GetGenericArguments().Length == 1 && typeof(IRepository<>) == i.GetGenericTypeDefinition())
                where type.IsInterface && genericApi != null
                select new { RepoApi = type, GenericApi = genericApi};

            foreach (var apiPair in repoApis)
            {
                Type repoProxy;
                // Find implementations for the RepoAPI, which will also fit the generic API because of the inheritance
                var implementations = types.Where(t => t.IsClass && apiPair.RepoApi.IsAssignableFrom(t)).ToList();
                if (implementations.Count == 0)
                {
                    repoProxy = ProxyBuilder.Build(apiPair.RepoApi);
                }
                else
                {
                    var selectedImpl = implementations.First();
                    repoProxy = ProxyBuilder.Build(apiPair.RepoApi, selectedImpl);
                }

                var constructorDelegate = ReflectionTool.ConstructorDelegate<Repository>(repoProxy);
                // Register constructor for both interfaces
                Repositories[apiPair.RepoApi] = Repositories[apiPair.GenericApi] = constructorDelegate;
            }
        }
    }
}