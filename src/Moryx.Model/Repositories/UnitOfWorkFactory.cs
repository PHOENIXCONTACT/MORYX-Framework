// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moryx.Model.Configuration;
using Moryx.Model.Repositories.Proxy;

namespace Moryx.Model.Repositories
{
    /// <inheritdoc />
    public sealed class UnitOfWorkFactory<TContext> : IUnitOfWorkFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Manager that does the real work
        /// </summary>
        private IDbContextManager _manager;

        private readonly RepositoryProxyBuilder _proxyBuilder = new RepositoryProxyBuilder();
        private readonly IList<Type> _repositories = new List<Type>();

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWorkFactory{TContext}"/>
        /// </summary>
        public UnitOfWorkFactory(IDbContextManager dbContextManager)
        {
            _manager = dbContextManager;
            RegisterRepositories();
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create()
        {
            var context = _manager.Create<TContext>();
            return new UnitOfWork<TContext>(context, _repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(IDatabaseConfig config)
        {
            var context = _manager.Create<TContext>(config);
            return new UnitOfWork<TContext>(context, _repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(ContextMode contextMode)
        {
            var context = _manager.Create<TContext>(contextMode);
            return new UnitOfWork<TContext>(context, _repositories);
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(IDatabaseConfig config, ContextMode contextMode)
        {
            var context = _manager.Create<TContext>(config, contextMode);
            return new UnitOfWork<TContext>(context, _repositories);
        }

        private void RegisterRepositories()
        {
            var types = typeof(TContext).Assembly.GetTypes();

            var repoApis = from type in types
                let repoApi = type.GetInterfaces().FirstOrDefault(i => i.GetGenericArguments().Length == 1 && typeof(IRepository<>) == i.GetGenericTypeDefinition())
                where type.IsInterface && repoApi != null
                select type;

            foreach (var repoApi in repoApis)
            {
                var implementations = types.Where(t => t.IsClass && repoApi.IsAssignableFrom(t)).ToList();
                if (implementations.Count == 0)
                {
                    var apiImpl = _proxyBuilder.Build(repoApi);
                    _repositories.Add(apiImpl);
                    continue;
                }

                var selectedImpl = implementations.First();
                var typeImpl = _proxyBuilder.Build(repoApi, selectedImpl);
                _repositories.Add(typeImpl);
            }
        }
    }
}