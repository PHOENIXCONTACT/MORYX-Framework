// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

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
        /// Get or set the current mode
        /// </summary>
        ContextMode Mode { get; set; }

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
