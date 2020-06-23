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
    public sealed class UnitOfWork : IUnitOfWork, IGenericUnitOfWork, IModelDiagnostics
    {
        private MoryxDbContext _context;
        private readonly IEnumerable<Type> _repositories;

        /// <inheritdoc />
        public ContextMode Mode
        {
            get { return _context.CurrentMode; }
            set { _context.Configure(value); }
        }

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWork"/>
        /// </summary>
        /// <param name="context">Responsible <see cref="DbContext"/></param>
        /// <param name="repoBuilders">Current availabe repositories</param>
        public UnitOfWork(MoryxDbContext context, IEnumerable<Type> repoBuilders)
        {
            _context = context;
            _repositories = repoBuilders;
        }

        /// <inheritdoc />
        public T GetRepository<T>() where T : class, IRepository
        {
            return (T) GetRepository(typeof(T));
        }

        /// <inheritdoc />
        IRepository IGenericUnitOfWork.GetRepository(Type api)
        {
            return GetRepository(api);
        }

        /// <inheritdoc />
        bool IGenericUnitOfWork.HasRepository(Type api)
        {
            return _repositories.Any(api.IsAssignableFrom);
        }

        /// <inheritdoc />
        Action<string> IModelDiagnostics.Log
        {
            get { return _context.Database.Log; }
            set { _context.Database.Log = value; }
        }

        private IRepository GetRepository(Type api)
        {
            var repoType = _repositories.SingleOrDefault(api.IsAssignableFrom);
            if (repoType == null)
            {
                throw new NotSupportedException($"Api {api} was not found.");
            }

            var instance = (Repository) Activator.CreateInstance(repoType);
            instance.Initialize(this, _context);
            
            return instance;
        }

        /// <inheritdoc />
        public void Save()
        {
            try
            {
                _context.SaveChanges();
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
                await _context.SaveChangesAsync(cancellationToken);
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
            if (_context == null)
                return;

            _context.Dispose();
            _context = null;
        }
    }
}
