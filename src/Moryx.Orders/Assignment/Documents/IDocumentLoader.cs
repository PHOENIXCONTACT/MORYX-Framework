// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Orders.Documents;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Plugin to load the needed documents
    /// </summary>
    public interface IDocumentLoader : IConfiguredPlugin<DocumentLoaderConfig>
    {
        /// <summary>
        /// Load document information for the given operation
        /// </summary>
        Task<IReadOnlyList<Document>> Load(Operation operation);
    }
}
