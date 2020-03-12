// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Marvin.Model
{
    /// <summary>
    /// Unit of work for db access with specialized generic apis
    /// </summary>
    public interface IGenericUnitOfWork
    {
        /// <summary>
        /// Creates an repository by the given API type
        /// </summary>
        IRepository GetRepository(Type api);

        /// <summary>
        /// Checks whether the specialized generic api exists
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        bool HasRepository(Type api);
    }

    /// <summary>
    /// Unit of work for db access
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
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
        void Save();

        /// <summary>
        /// Asynchronously saves all changes made in this UnitOfWork to the underlying database.
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// Asynchronously saves all changes made in this UnitOfWork to the underlying database.
        /// </summary>
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
