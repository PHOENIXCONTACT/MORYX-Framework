// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// Links an entity to a business object. When calling uow.SaveChanged(), the unit of work
        /// will update the IDs of all linke businessObjects 
        /// </summary>
        /// <param name="businessObject"></param>
        /// <param name="entity"></param>
        void LinkEntityToBusinessObject(IPersistentObject businessObject, IEntity entity);
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
