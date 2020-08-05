// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Model.Repositories
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly IEnumerable<Type> _repositories;

        /// <inheritdoc />
        public TContext DbContext { get; }

        /// <inheritdoc />
        DbContext IUnitOfWork.DbContext => DbContext;

        /// <inheritdoc />
        public ContextMode Mode
        {
            get => DbContext.GetContextMode();
            set => DbContext.SetContextMode(value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>
        /// </summary>
        /// <param name="dbContext">Responsible <see cref="System.Data.Entity.DbContext"/></param>
        /// <param name="repositories">Current available repositories</param>
        public UnitOfWork(TContext dbContext, IEnumerable<Type> repositories)
        {
            DbContext = dbContext;
            _repositories = repositories;
        }

        /// <inheritdoc />
        public T GetRepository<T>() where T : class, IRepository
        {
            return (T) GetRepository(typeof(T));
        }

        private IRepository GetRepository(Type api)
        {
            var repoType = _repositories.SingleOrDefault(api.IsAssignableFrom);
            if (repoType == null)
            {
                throw new NotSupportedException($"Api {api} was not found.");
            }

            var instance = (Repository) Activator.CreateInstance(repoType);
            instance.Initialize(this, DbContext);

            return instance;
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            try
            {
                DbContext.SaveChanges();
            }
            // Catch for validation error break point
            catch (DbEntityValidationException valEx)
            {
                // ReSharper disable once UnusedVariable
                var validationError = valEx.EntityValidationErrors;
                throw;
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
        public Task SaveChangesAsync() =>
            SaveChangesAsync(CancellationToken.None);

        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            // Catch for validation error break point
            catch (DbEntityValidationException valEx)
            {
                // ReSharper disable once UnusedVariable
                var validationError = valEx.EntityValidationErrors;
                throw;
            }
            // Catch for other exception break points
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                // Debug entity framework exceptions
                throw;
            }
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
