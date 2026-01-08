// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData
{
    /// <summary>
    /// Generic interface for components publishing process data
    /// </summary>
    public interface IProcessDataPublisher
    {
        /// <summary>
        /// Will be raised if process data is occurred
        /// </summary>
        event EventHandler<Measurement> ProcessDataOccurred;
    }
}