// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Class for an emtpy assignement of product parts. Implements <see cref="IPartsAssignment"/>
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IPartsAssignment), Name = nameof(NullPartsAssignment))]
    public class NullPartsAssignment : IPartsAssignment
    {
        /// <inheritdoc />
        public Task InitializeAsync(PartsAssignmentConfig config)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Always returns an empty parts list
        /// </summary>
        public Task<IReadOnlyList<ProductPart>> LoadPartsAsync(Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken)
            => Task.FromResult(operation.Parts ?? (IReadOnlyList<ProductPart>)Enumerable.Empty<ProductPart>());
    }
}
