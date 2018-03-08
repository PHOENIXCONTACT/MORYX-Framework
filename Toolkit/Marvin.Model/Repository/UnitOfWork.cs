using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace Marvin.Model
{
    /// <summary>
    /// Unit of work 
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork, IGenericUnitOfWork
    {
        private MarvinDbContext _context;
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
        public UnitOfWork(MarvinDbContext context, IEnumerable<Type> repoBuilders)
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

        private IRepository GetRepository(Type api)
        {
            var repoType = _repositories.Single(api.IsAssignableFrom);

            var instance = (Repository)Activator.CreateInstance(repoType);
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
                var validationError = valEx.EntityValidationErrors;
                throw;
            }
            // Catch for other exception break points
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