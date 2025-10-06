// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Connector of one or more transitions
    /// </summary>
    public interface IConnector : IWorkplanNode
    {
        /// <summary>
        /// Classification of this connector
        /// </summary>
        NodeClassification Classification { get; set; }

        /// <summary>
        /// Create a place instance
        /// </summary>
        IPlace CreateInstance();
    }
}
