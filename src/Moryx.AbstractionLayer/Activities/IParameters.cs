// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Common interface for all parameters
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Create new parameters object with resolved binding values from process
        /// </summary>
        IParameters Bind(IProcess process);
    }
}
