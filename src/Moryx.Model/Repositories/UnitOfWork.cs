// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly IDictionary<Type, Func<Repository>> _repositories;

        /// <inheritdoc />
        public TContext DbContext { get; }

        /// <inheritdoc />
        DbContext IUnitOfWork.DbContext => DbContext;

        private readonly Dictionary<IPersistentObject, IEntity> _entityBusinessObjectLinks = new();

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>
        /// </summary>
        /// <param name="dbContext">Responsible <see cref="Microsoft.EntityFrameworkCore.DbContext"/></param>
        /// <param name="repositories">Current available repositories</param>
        public UnitOfWork(TContext dbContext, IDictionary<Type, Func<Repository>> repositories)
        {
            DbContext = dbContext;
            _repositories = repositories;
        }

        /// <inheritdoc />
        public T GetRepository<T>() where T : class, IRepository
        {
            return (T)GetRepository(typeof(T));
        }

        private IRepository GetRepository(Type api)
        {
            if (!_repositories.ContainsKey(api))
            {
                throw new NotSupportedException($"Api {api} was not found.");
            }

            var instance = _repositories[api]();
            instance.Initialize(this, DbContext);

            return instance;
        }

        /// <inheritdoc />
        public void LinkEntityToBusinessObject(IPersistentObject businessObject, IEntity entity)
        {
            _entityBusinessObjectLinks[businessObject] = entity;
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            try
            {
                DbContext.SaveChanges();
            }
            // Catch for other exception break points
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                // Debug entity framework exceptions
                throw;
            }

            foreach (var link in _entityBusinessObjectLinks)
            {
                link.Key.Id = link.Value.Id;
            }
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            // Catch for other exception break points
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                // Debug entity framework exceptions
                throw;
            }
        }

        /// <inheritdoc />
        public bool IsLinked(IPersistentObject businessObject)
        {
            return _entityBusinessObjectLinks.ContainsKey(businessObject);
        }

        private bool _disposed;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseContext();
                }
            }
            _disposed = true;
        }

        private void CloseContext()
        {
            DbContext?.Dispose();
        }
    }
}
