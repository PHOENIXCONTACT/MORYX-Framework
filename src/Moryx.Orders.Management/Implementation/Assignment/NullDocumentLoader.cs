// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
        public void Initialize(DocumentLoaderConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        public void Stop()
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Document>> Load(Operation operation)
            => Task.FromResult(EmptyDocuments);
    }
}
