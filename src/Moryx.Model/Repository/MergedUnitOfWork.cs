// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moryx.Tools;

namespace Moryx.Model
{
    /// <summary>
    /// <see cref="IUnitOfWork"/> wrapper which will be used for parent - child relations
    /// </summary>
    public sealed class MergedUnitOfWork : IUnitOfWork, IGenericUnitOfWork, IModelDiagnostics
    {
        private IUnitOfWork[] _unitOfWorks;
        private Action<string> _diagnosticAction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public MergedUnitOfWork(IUnitOfWork parent, IUnitOfWork child)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (child == null)
                throw new InvalidOperationException("No child given");

            _unitOfWorks = new[] {parent, child};
        }

        /// <inheritdoc />
        public T GetRepository<T>() where T : class, IRepository => 
            (T) GetRepository(typeof(T));

        /// <inheritdoc />
        IRepository IGenericUnitOfWork.GetRepository(Type api) => 
            GetRepository(api);

        private IRepository GetRepository(Type api)
        {
            var target = _unitOfWorks.FirstOrDefault(uow => ((IGenericUnitOfWork)uow).HasRepository(api));
            if (target == null)
                throw new NotSupportedException($"Api {api} was not found.");

            return ((IGenericUnitOfWork)target).GetRepository(api);
        }

        /// <inheritdoc />
        bool IGenericUnitOfWork.HasRepository(Type api) => 
            _unitOfWorks.Any(uow => ((IGenericUnitOfWork)uow).HasRepository(api));

        /// <summary>
        /// Sets and returns the mode for all contexts
        /// </summary>
        public ContextMode Mode
        {
            get { return _unitOfWorks.Aggregate(ContextMode.AllOff, (current, uow) => current | uow.Mode); }
            set { _unitOfWorks.ForEach(uow => uow.Mode = value); }
        }

        /// <inheritdoc />
        public void Save()
        {
            foreach (var uow in _unitOfWorks)
                uow.Save();
        }

        /// <inheritdoc />
        public Task SaveAsync() => 
            SaveAsync(CancellationToken.None);

        /// <inheritdoc />
        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            foreach (var uow in _unitOfWorks)
                await uow.SaveAsync(cancellationToken);
        }

        /// <inheritdoc />
        Action<string> IModelDiagnostics.Log
        {
            get { return _diagnosticAction; }
            set
            {
                _diagnosticAction = value;
                _unitOfWorks.ForEach(uow => ((IModelDiagnostics)uow).Log = value);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _unitOfWorks.ForEach(uow => uow.Dispose());
            _unitOfWorks = null;
        }
    }
}
