// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;
using Moryx.Orders.Documents;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Default loader of there is no document loader configured
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IDocumentLoader), Name = nameof(NullDocumentLoader))]
    internal class NullDocumentLoader : IDocumentLoader
    {
        private static readonly IReadOnlyList<Document> EmptyDocuments = Array.Empty<Document>();

        /// <inheritdoc />
        public Task InitializeAsync(DocumentLoaderConfig config)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Document>> LoadAsync(Operation operation, CancellationToken cancellationToken = default)
            => Task.FromResult(EmptyDocuments);
    }
}
