// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Model
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork, IGenericUnitOfWork, IModelDiagnostics, IDbContextHolder
    {
        private readonly IEnumerable<Type> _repositories;

        /// <inheritdoc />
        public DbContext DbContext { get; private set; }

        /// <inheritdoc />
        public ContextMode Mode
        {
            get => DbContext.GetContextMode();
            set => DbContext.SetContextMode(value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWork"/>
        /// </summary>
        /// <param name="dbContext">Responsible <see cref="System.Data.Entity.DbContext"/></param>
        /// <param name="repoBuilders">Current available repositories</param>
        public UnitOfWork(DbContext dbContext, IEnumerable<Type> repoBuilders)
        {
            DbContext = dbContext;
            _repositories = repoBuilders;
        }

        /// <inheritdoc />
        public T GetRepository<T>() where T : class, IRepository => (T) GetRepository(typeof(T));

        /// <inheritdoc />
        IRepository IGenericUnitOfWork.GetRepository(Type api) => GetRepository(api);

        /// <inheritdoc />
        bool IGenericUnitOfWork.HasRepository(Type api) => _repositories.Any(api.IsAssignableFrom);

        /// <inheritdoc />
        Action<string> IModelDiagnostics.Log
        {
            get => DbContext.Database.Log;
            set => DbContext.Database.Log = value;
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
        public void Save()
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
        public Task SaveAsync() =>
            SaveAsync(CancellationToken.None);

        /// <inheritdoc />
        public async Task SaveAsync(CancellationToken cancellationToken)
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
            if (DbContext == null)
                return;

            DbContext.Dispose();
            DbContext = null;
        }
    }
}
