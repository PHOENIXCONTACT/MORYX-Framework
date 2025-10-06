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
        public void Initialize(PartsAssignmentConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <summary>
        /// Always returns an empty parts list
        /// </summary>
        public Task<IEnumerable<ProductPart>> LoadParts(Operation operation, IOperationLogger operationLogger) 
            => Task.FromResult(operation.Parts ?? Enumerable.Empty<ProductPart>());
    }
}
