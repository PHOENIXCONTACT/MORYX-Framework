using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Extensions to the EF DbContext
    /// </summary>
    public static class DbContextExtensions
    {
        private static readonly Dictionary<Type, Type> Repositories = new Dictionary<Type, Type>();
        private static readonly RepositoryProxyBuilder ProxyBuilder = new RepositoryProxyBuilder();

        public static TApi GetRepository<TApi>(this DbContext dbContext) where TApi : class, IRepository
        {
            var apiType = typeof(TApi);

            var implType = Repositories.ContainsKey(apiType)
                ? Repositories[apiType]
                : Repositories[apiType] = ProxyBuilder.Build(typeof(TApi));

            return Instantiate<TApi>(dbContext, implType);
        }

        public static TApi GetRepository<TApi, TImpl>(this DbContext dbContext) where TApi : class, IRepository
        {
            return GetRepository<TApi, TImpl>(dbContext, false);
        }

        public static TApi GetRepository<TApi, TImpl>(this DbContext dbContext, bool noProxy) where TApi : class, IRepository
        {
            var apiType = typeof(TApi);

            var implType = Repositories.ContainsKey(apiType)
                ? Repositories[apiType]
                : Repositories[apiType] = noProxy ? typeof(TImpl) : ProxyBuilder.Build(apiType, typeof(TImpl));

            return Instantiate<TApi>(dbContext, implType);
        }

        private static TApi Instantiate<TApi>(DbContext dbContext, Type implType)
        {
            var repoInstance = (IInitializableRepository)Activator.CreateInstance(implType, dbContext);
            repoInstance.Initialize(dbContext);

            return (TApi)repoInstance;
        }
    }
}
