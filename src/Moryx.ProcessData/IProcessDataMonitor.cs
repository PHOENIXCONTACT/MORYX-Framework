// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData
{
    /// <summary>
    /// Central facade to collect process data. Will be implemented by the ProcessDataMonitor module
    /// </summary>
    public interface IProcessDataMonitor
    {
        /// <summary>
        /// Adds a single measurement
        /// </summary>
        void Add(Measurement measurement);

        /// <summary>
        /// Adds a full measurand which is containing multiple measurements
        /// </summary>
        void Add(Measurand measurand);
    }
}