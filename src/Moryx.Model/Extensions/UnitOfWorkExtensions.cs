// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model
{
    /// <summary>
    /// A collection of static methods to be used with database repositories.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Get or create an entity for a business object
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity GetEntity<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var entity = unitOfWork.FindEntity<TEntity>(obj);

            entity ??= unitOfWork.CreateEntity<TEntity>(obj);

            return entity;
        }

        /// <summary>
        /// Get or create an entity for a business object async
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static async Task<TEntity> GetEntityAsync<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var entity = unitOfWork.FindEntity<TEntity>(obj);

            entity ??= await unitOfWork.CreateEntityAsync<TEntity>(obj);

            return entity;
        }

        /// <summary>
        /// Get an entity for a business object or return null
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity FindEntity<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var repository = unitOfWork.GetRepository<IRepository<TEntity>>();
            var entity = repository.GetByKey(obj.Id);

            return entity;
        }

        /// <summary>
        /// Get an entity for a business object async or return null
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static async Task<TEntity> FindEntityAsync<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var repository = unitOfWork.GetRepository<IRepository<TEntity>>();
            var entity = await repository.GetByKeyAsync(obj.Id);

            return entity;
        }

        /// <summary>
        /// Create an entity for a business object
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static TEntity CreateEntity<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var repository = unitOfWork.GetRepository<IRepository<TEntity>>();

            var entity = repository.Create();
            unitOfWork.LinkEntityToBusinessObject(obj, entity);

            return entity;
        }

        /// <summary>
        /// Create an entity for a business object async
        /// </summary>
        /// <param name="unitOfWork">An open database unit of work</param>
        /// <param name="obj">The business object</param>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        public static async Task<TEntity> CreateEntityAsync<TEntity>(this IUnitOfWork unitOfWork, IPersistentObject obj)
            where TEntity : class, IEntity
        {
            var repository = unitOfWork.GetRepository<IRepository<TEntity>>();

            var entity = await repository.CreateAsync();
            unitOfWork.LinkEntityToBusinessObject(obj, entity);

            return entity;
        }

    }
}
