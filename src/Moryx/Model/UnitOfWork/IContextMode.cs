// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model
{
    /// <summary>
    /// Interface to configure the current database context mode
    /// </summary>
    public interface IContextMode
    {
        /// <summary>
        /// Returns the cureent context mode of the DbContext
        /// </summary>
        ContextMode CurrentMode { get; }

        /// <summary>
        /// Sets the new ContextMode on the DbContext
        /// </summary>
        void Configure(ContextMode mode);
    }
}
