// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Unit of work for db access
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Underlying database context of this unit of work
        /// </summary>
        DbContext DbContext { get; }

        /// <summary>
        /// Call to internal container for repo implementation
        /// </summary>
        T GetRepository<T>() where T : class, IRepository;

        /// <summary>
        /// Saves all changes made in this UnitOfWork to the underlying database.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this UnitOfWork to the underlying database.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Asynchronously saves all changes made in this UnitOfWork to the underlying database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Links an entity to a business object. When calling <see cref="IUnitOfWork.SaveChanges()"/>, the unit of work
        /// will update the IDs of all linked business objects 
        /// </summary>
        /// <param name="businessObject">The business object to update on <see cref="IUnitOfWork.SaveChanges()"/></param>
        /// <param name="entity">The entity saved with <see cref="IUnitOfWork.SaveChanges()"/></param>
        void LinkEntityToBusinessObject(IPersistentObject businessObject, IEntity entity);

        /// <summary>
        /// Checks whether the given business object is linked to an entity to update its <seealso cref="IPersistentObject.Id"/>
        /// when calling <see cref="IUnitOfWork.SaveChanges()"/>.
        /// </summary>
        /// <param name="businessObject">The business object to check</param>
        bool IsLinked(IPersistentObject businessObject);
    }

    /// <summary>
    /// Unit of work with more typed database context
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IUnitOfWork<out TContext> : IUnitOfWork
        where TContext : DbContext
    {
        /// <summary>
        /// Underlying database context of this unit of work
        /// </summary>
        new TContext DbContext { get; }
    }
}
